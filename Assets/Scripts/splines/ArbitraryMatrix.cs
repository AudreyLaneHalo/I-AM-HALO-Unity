using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
    public class ArbitraryMatrix
    {
        private float[][] matrix;

        public float[] this[int i]
        {
            get
            {
                if (i >= 0 && i < rows)
                {
                    return matrix[i];
                }
                Debug.LogWarning("Cannot return row: index " + i + " is out of range " + rows);
                return new float[columns];
            }
            set
            {
                if (value.Length == columns)
                {
                    matrix[i] = value;
                }
                else
                {
                    Debug.LogWarning("Cannot set row: " + value.Length + " is not equal to column size " + columns);
                }
            }
        }

        public int rows
        {
            get
            {
                return matrix.Length;
            }
        }

        public int columns
        {
            get
            {
                return matrix[0].Length;
            }
        }

        public ArbitraryMatrix(int _rows, int _columns)
        {
            if (_rows > 0 && _columns > 0)
            {
                matrix = new float[_rows][];
                for (int r = 0; r < rows; r++)
                {
                    matrix[r] = new float[_columns];
                }
            }
            else
            {
                Debug.LogError("Matrix dimensions need to be greater than 0");
            }
        }

        public override string ToString()
        {
            string s = "[ ";
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    s += matrix[r][c] + " ";
                }
                if (r < rows - 1) { s += ", "; }
            }
            return s + "]";
        }

        public static ArbitraryMatrix operator *(ArbitraryMatrix matrix1, ArbitraryMatrix matrix2)
        {
            if (matrix1.rows != matrix2.rows || matrix1.columns != matrix2.columns)
            {
                Debug.LogWarning("Cannot multiply matrices: matrices must have same dimensions");
                return matrix1;
            }

            ArbitraryMatrix solution = new ArbitraryMatrix(matrix1.rows, matrix2.columns);
            for (int r = 0; r < solution.rows; r++)
            {
                for (int c = 0; c < solution.columns; c++)
                {
                    float sum = 0;
                    for (int k = 0; k < solution.rows; k++)
                    {
                        sum += matrix1[r][k] * matrix2[k][c];
                    }
                    solution[r][c] = sum;
                }
            }
            return solution;
        }

        public static float[] operator *(ArbitraryMatrix _matrix, float[] vector)
        {
            if (vector.Length != _matrix.columns)
            {
                Debug.LogWarning("Cannot solve matrix equation: vector length is not equal to matrix columns");
                return vector;
            }

            float[] solution = new float[_matrix.columns];
            for (int r = 0; r < _matrix.rows; r++)
            {
                for (int c = 0; c < _matrix.columns; c++)
                {
                    solution[r] += _matrix[r][c] * vector[c];
                }
            }
            return solution;
        }

        public static ArbitraryMatrix operator *(float scalar, ArbitraryMatrix _matrix)
        {
            ArbitraryMatrix m = new ArbitraryMatrix(_matrix.rows, _matrix.columns);
            for (int r = 0; r < _matrix.rows; r++)
            {
                for (int c = 0; c < _matrix.columns; c++)
                {
                    m[r][c] = scalar * _matrix[r][c];
                }
            }
            return m;
        }

        public ArbitraryMatrix inverse
        {
            get
            {
                float d = determinant;
                if (d == 0)
                {
                    Debug.LogWarning("Cannot calculate inverse of matrix with determinant = 0");
                    return this;
                }

                ArbitraryMatrix result = (1f / d) * coFactor.transpose;
                return result;
            }
        }

        public float determinant
        {
            get
            {
                return CalculateDeterminant(this);
            }
        }

        float CalculateDeterminant(ArbitraryMatrix _matrix)
        {
            float d = 0;
            if (_matrix.rows != _matrix.columns)
            {
                Debug.LogWarning("Cannot calculate determinant of non-square matrix");
                return d;
            }

            if (_matrix.rows < 1)
            {
                return 0;
            }
            else if (_matrix.rows == 1)
            {
                return _matrix[0][0];
            }
            else if (_matrix.rows == 2)
            {
                return (_matrix[0][0] * _matrix[1][1]) - (_matrix[1][0] * _matrix[0][1]);
            }
            else
            {
                for (int c = 0; c < _matrix.columns; c++)
                {
                    d += (Mathf.Pow(-1f, (float)c) * _matrix[0][c] * CalculateDeterminant(_matrix.SubMatrix(0, c)));
                }
                return d;
            }
        }

        public ArbitraryMatrix coFactor
        {
            get
            {
                if (rows != columns || rows < 2)
                {
                    Debug.LogWarning("Cannot calculate coFactor matrix of non-square matrix or matrix with dimensions less than 2");
                    return this;
                }

                ArbitraryMatrix cf = new ArbitraryMatrix(rows, columns);

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        cf[r][c] = Mathf.Pow(-1f, (float)(r + c)) * SubMatrix(r, c).determinant;
                    }
                }
                return cf;
            }
        }

        ArbitraryMatrix SubMatrix(int rowToRemove, int columnToRemove)
        {
            ArbitraryMatrix sm = new ArbitraryMatrix(rows - 1, columns - 1);

            int k = 0, l = 0;
            for (int r = 0; r < rows; r++)
            {
                if (r != rowToRemove)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        if (c != columnToRemove)
                        {
                            sm[l][k] = matrix[r][c];

                            k = (k + 1) % (rows - 1);
                            if (k == 0)
                            {
                                l++;
                            }
                        }
                    }
                }
            }
            return sm;
        }

        public ArbitraryMatrix transpose
        {
            get
            {
                if (rows != columns)
                {
                    Debug.LogWarning("Cannot calculate transpose of non-square matrix");
                    return this;
                }

                ArbitraryMatrix t = new ArbitraryMatrix(rows, columns);

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        t[r][c] = matrix[c][r];
                        t[c][r] = matrix[r][c];
                    }
                }
                return t;
            }
        }
    }
}