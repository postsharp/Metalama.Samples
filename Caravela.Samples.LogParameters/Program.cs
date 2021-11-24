internal class Program
{
    private static void Main()
    {
        Add(1, 1);
    }

    [Log]
    private static int Add(int a, int b)
    {
        return a + b;
    }
}