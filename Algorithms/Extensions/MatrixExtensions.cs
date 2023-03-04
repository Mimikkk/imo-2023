namespace Algorithms.Extensions;

public static class MatrixExtensions {
  public static T[] ReadRow<T>(this T[,] matrix, int row) {
    var result = new T[matrix.GetLength(1)];
    for (var i = 0; i < matrix.GetLength(1); i++) result[i] = matrix[row, i];

    return result;
  }

  public static T[] ReadColumn<T>(this T[,] matrix, int column) {
    var result = new T[matrix.GetLength(0)];
    for (var i = 0; i < matrix.GetLength(0); i++) result[i] = matrix[i, column];

    return result;
  }
}
