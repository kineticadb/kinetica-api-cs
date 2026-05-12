# Wire-Format Pinning Tests

## Overview

This directory contains comprehensive wire-format pinning tests to ensure backward compatibility of the Kinetica C# API's binary protocols. These tests use **pinned test vectors** (hardcoded expected byte arrays) to detect any unintended changes to the wire format.

## Purpose

Wire-format tests serve three critical purposes:

1. **Backward Compatibility**: Ensure that protocol changes don't break compatibility with existing Kinetica servers
2. **Regression Detection**: Catch unintended encoding changes during refactoring or optimization
3. **Documentation**: Serve as executable specification of the binary protocol

## Test Files

### 1. `RecordKeyDecimalTests.cs`
**Category**: `[WireFormat]`, `[RecordKey]`

Tests the binary encoding of decimal values in RecordKeys (used for shard routing).

**Critical Wire Formats**:
- 8-byte decimals (precision ≤ 18): Little-endian int64
- 12-byte decimals (precision > 18): Little-endian 96-bit integer
- Precision boundary at 18/19

**Why it matters**: RecordKey encoding determines shard routing. Changes break data distribution.

**Test vectors**:
```csharp
"123.45" with scale 4 → 1234500 as int64
"-987.6543" with scale 4 → -9876543 as int64 (two's complement)
```

### 2. `AvroWireFormatTests.cs`
**Category**: `[WireFormat]`

Tests the Avro binary encoding for all primitive types.

**Covered encodings**:
- **Varint (zig-zag)**: Integers and longs
  - `0 → 0x00`, `1 → 0x02`, `-1 → 0x01`
  - `int.MaxValue → 0xFE, 0xFF, 0xFF, 0xFF, 0x0F`
- **Fixed-size**: Floats (4 bytes LE), Doubles (8 bytes LE)
- **Booleans**: `true → 0x01`, `false → 0x00`
- **Strings**: Varint length + UTF-8 bytes
  - `"test" → 0x08, 0x74, 0x65, 0x73, 0x74`
- **Nullable types**: Union encoding with index byte

**Test vectors** (examples):
```csharp
int: 42 → 0x54
long: 1000 → 0xD0, 0x0F
float: 3.14f → 0xC3, 0xF5, 0x48, 0x40 (IEEE 754 LE)
double: 2.718 → 0xB2, 0x9D, 0xEF, 0xA7, 0xC6, 0xB7, 0x05, 0x40
string: "Hello™" → 0x10, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xE2, 0x84, 0xA2
```

### 3. `ProtocolWireFormatTests.cs`
**Category**: `[WireFormat]`, `[Protocol]`

Tests higher-level protocol encoding including RecordKeys, type schemas, and compression.

**Covered areas**:
- RecordKey encoding for different types (int, long, string, decimal)
- Composite shard keys (field order)
- Type schema stability
- Column property constants
- Compression round-tripping

## Running Wire-Format Tests

```bash
# Run all wire-format tests
dotnet test --filter "Category=WireFormat"

# Run specific test file
dotnet test --filter "FullyQualifiedName~AvroWireFormatTests"

# Run RecordKey tests only
dotnet test --filter "Category=RecordKey"
```

## Understanding Test Failures

### ⚠️ Test Failure = Potential Breaking Change

If a wire-format test fails, it means:

1. **The binary encoding has changed** - This could break compatibility
2. **Action required**:
   - If intentional: Update the test vectors AND document the breaking change
   - If unintentional: Fix the regression

### Example Failure

```
Assert.Equal() Failure: Collections differ
         ↓ (pos 2)
Expected: [0x54, 0xD0, 0x0F, ···]
Actual:   [0x54, 0xA0, 0x07, ···]
         ↑ (pos 1)
```

This indicates the encoding of a varint changed at byte position 1.

## Maintaining Tests

### Adding New Wire-Format Tests

When adding a new encoding:

1. Create a test with a known input value
2. Encode it and observe the actual bytes
3. **Verify correctness** (don't just copy observed bytes!)
4. Pin the expected bytes in the test
5. Add documentation explaining the encoding

Example template:
```csharp
/// <summary>
/// WIRE FORMAT TEST: [Brief description]
///
/// Test vector: [input value]
/// Expected encoding: [hex bytes]
///
/// This encoding MUST remain stable for [reason].
/// </summary>
[Fact]
public void MyWireFormatTest()
{
    // Arrange: Input value
    var input = ...;

    // Expected bytes (PINNED - do not change without review)
    byte[] expected = { 0xAB, 0xCD, ... };

    // Act: Encode
    byte[] actual = Encode(input);

    // Assert: Exact match
    Assert.Equal(expected, actual);
}
```

### Updating Test Vectors

**ONLY update test vectors when**:
1. The change is intentional and reviewed
2. You've verified the new encoding is correct
3. You've documented the breaking change

**Process**:
1. Update the expected bytes
2. Add a comment explaining why: `// Updated 2026-04-22: Changed varint encoding to...`
3. Update CHANGELOG.md with breaking change notice
4. Review with team

## Avro Encoding Reference

### Varint (Zig-Zag) Encoding

Used for: `int`, `long`

Algorithm:
```csharp
// Encode signed to unsigned (zig-zag)
ulong n = (ulong)((value << 1) ^ (value >> 63));

// Write as base-128 varint
while ((n & ~0x7FUL) != 0) {
    buffer[pos++] = (byte)((n & 0x7F) | 0x80);
    n >>= 7;
}
buffer[pos++] = (byte)n;
```

### String Encoding

```
[varint length][UTF-8 bytes...]
```

Example: `"test"` (4 bytes)
- Length: 4 → varint: `0x08` (zig-zag: 4×2=8)
- Bytes: `0x74 0x65 0x73 0x74`

### Union Encoding (Nullable Types)

```
[varint union_index][value if non-null]
```

Example: `int? value = 42`
- Union type: `["null", "int"]`
- Non-null index: 1 → `0x02`
- Value: 42 → `0x54`
- Result: `0x02 0x54`

## Resources

- [Apache Avro Specification](https://avro.apache.org/docs/current/spec.html)
- Kinetica Wire Protocol: `docs/protocol.md`
- RecordKey Implementation: `Kinetica/Utils/RecordKey.cs`
- Avro Encoders: `Kinetica/BulkInserter/AvroEncoders.cs`

## Test Coverage Status

| Component | Coverage | Status |
|-----------|----------|--------|
| Varint encoding (int/long) | ✅ Full | 15 test vectors |
| Primitive types (float/double/bool) | ✅ Full | Basic values + edge cases |
| String encoding | ✅ Good | ASCII + UTF-8 multi-byte |
| RecordKey decimals | ✅ Full | 8-byte + 12-byte formats |
| RecordKey composite keys | ✅ Basic | Int+string combinations |
| Nullable types | ✅ Basic | Null + non-null cases |
| Type schemas | ⚠️ Partial | Basic structure only |
| Request/Response payloads | ❌ None | Future enhancement |

## Future Enhancements

1. **Request/Response Tests**: Add full protocol message encoding tests
2. **Schema Evolution**: Test schema compatibility (field addition/removal)
3. **Binary Compatibility Matrix**: Test across multiple Kinetica versions
4. **Golden Files**: Store large test vectors in separate files
5. **Fuzz Testing**: Random inputs to verify encoder robustness

---

**Last Updated**: 2026-04-22
**Maintainer**: Engineering Team
**Review Frequency**: Every major release
