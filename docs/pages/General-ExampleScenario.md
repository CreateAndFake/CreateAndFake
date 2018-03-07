# Example Scenario

Imagine you have a class with a ton of data that you've marked with ISerializable. How do you verify that it, you know, actually serializes properly? You'd have to create an instance, populate it with data (which might be no small feat), then serialize it. But wait, are you sure the data actually serialized? You'll need to deserialize it, and probably create equality functions to handle figuring out if your data did in fact make it.

At the end of this you'll have a rather large test that you spent more time setting up than actually testing behavior, and then you have issues. The test has to change every time you change a property on the data class. What happens when a developer forgets to update the test when they added a new property? Potentially, your class now doesn't work correctly and your test is still green. At base, you only know your code works with that specific instance of canned data.

With those considerations in mind, I present to you the solution:

```c#
/// <summary>Verifies data can roundtrip.</summary>
[TestMethod]
public void DataHolder_BinarySerializes()
{
    DataHolder original = Tools.Randomizer.Create<DataHolder>();
    IFormatter formatter = new BinaryFormatter();

    using (Stream stream = new MemoryStream())
    {
        formatter.Serialize(stream, original);
        stream.Seek(0, SeekOrigin.Begin);

        Tools.Asserter.ValuesEqual(original, formatter.Deserialize(stream));
    }
}
```
