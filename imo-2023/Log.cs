namespace imo_2023;

public static class Log
{
  public static void L(object any) => WriteLine(any);

  public static void L(int[,] matrix)
  {
    WriteLine('[');
    for (var i = 0; i < matrix.GetLength(0); i++)
    {
      for (var j = 0; j < matrix.GetLength(1); j++)
      {
        Write(matrix[i, j]);
        if (j < matrix.GetLength(1) - 1) Write(", ");
      }

      WriteLine(';');
    }

    WriteLine(']');
  }
}
