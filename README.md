# Range-Primitive-Unity3D

A primitive type for creating restricted ranges for values in Unity.

## Features

- range primitive type
- custom Unity property drawer
- extension methods for various math operations

Currently supports:

`int`, `float`, `Vector2`, `Vector2Int`, `Vector3`, `Vector3Int`

## Usage and Examples

### Creation

The range constructor takes two arguments:
- `min`: The minimum value (lower boundary) of the range
- `max`: The maximum value (upper boundary) of the range

Creating a new range object.

```csharp
Range<int> myRange = new Range<int>(1, 5);
```

Creating a new range object as a Unity field.

```csharp
using RangePrimitive;
using UnityEngine;

public class Foo : MonoBehaviour
{
    [SerializeField]
    private Range<int> myRange;
}
```

### Usage Examples

One-dimensional range.

```csharp
// Generate 5 equally distributed samples between two values
Range<int> range = new Range<int>(12, 168);

for (int i = 0; i <= 4; i++)
{
    Debug.Log(range.Lerp(i / 4f)); // Output: 12, 51, 90, 129, 168
}

// Get the spread of the defined range
Debug.Log(range.Size()); // Output: 156

// Calculate at what point the value 37 lies between the start and end of the range
Debug.Log(range.InverseLerp(37)); // Output: 0.16
```

Two-dimensional range.

```csharp
// Generate 3 random points in a defined 2D area
Range<Vector2> range = new Range<Vector2>(new Vector2(-10, -10), new Vector2(10, 10));

for (int i = 0; i < 3; i++)
{
    Debug.Log(range.Random()); // Example output: (2.59, 4.79), (-4.82, 8.09), (8.21, 9.96)
}

// Check whether a point lies within the defined area
Debug.Log(range.Contains(new Vector2(-8.3f, 15f))); // Output: False
```

## Installation

The package can be installed using [OpenUPM](https://openupm.com/packages/com.iristrummer.range-primitive)
1. Install the [OpenUPM CLI](https://github.com/openupm/openupm-cli#installation)
2. Run the following command in the command line in your project directory:
`openupm add com.iristrummer.range-primitive`