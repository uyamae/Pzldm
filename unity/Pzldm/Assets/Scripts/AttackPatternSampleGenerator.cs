using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    public static class AttackPatternSampleGenerator
    {
        /// <summary>
        /// セル表示内容
        /// </summary>
        public enum CellType
        {
            /// <summary>
            /// 無し
            /// </summary>
            None,
            /// <summary>
            /// こうげきだま赤
            /// </summary>
            Red,
            /// <summary>
            /// こうげきだま青
            /// </summary>
            Blue,
            /// <summary>
            /// こうげきだま緑
            /// </summary>
            Green,
            /// <summary>
            /// こうげきだま黄
            /// </summary>
            Yellow,
            /// <summary>
            /// 一列落下
            /// </summary>
            DropColumn,
            /// <summary>
            /// １ライン落下
            /// </summary>
            DropRow,
            /// <summary>
            /// せり上げ
            /// </summary>
            LiftUp,
        }
        class Context
        {
            /// <summary>
            /// ピクセル幅
            /// </summary>
            public int Width { get; set; }
            /// <summary>
            /// ピクセル高さ
            /// </summary>
            public int Height { get; set; }
            /// <summary>
            /// セル１つのピクセルサイズ
            /// </summary>
            public int CellSize { get; set; }
            /// <summary>
            /// セルの列数
            /// </summary>
            public int ColumnsCount {  get; set; }
            public int RowsCount { get; set; }
            public int Margin { get; set; }
            private int[] lineNumbers;
            private CellType[,] cells;

            private static Color32 white = new Color32(255, 255, 255, 255);
            private static Color32 black = new Color32(0, 0, 0, 255);
            private static Color32 red = new Color32(255, 0, 0, 255);
            private static Color32 blue = new Color32(0, 0, 255, 255);
            private static Color32 green = new Color32(0, 255, 0, 255);
            private static Color32 yellow = new Color32(255, 255, 0, 255);
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="columnsCount"></param>
            public Context(int width, int height, int columnsCount)
            {
                Width = width;
                Height = height;
                CellSize = width / columnsCount;
                Margin = (width - CellSize * columnsCount) / 2;
                ColumnsCount = columnsCount;
                RowsCount = height / CellSize;
                lineNumbers = new int[RowsCount];
                cells = new CellType[ColumnsCount, RowsCount];
            }
            /// <summary>
            /// 一段落下
            /// </summary>
            /// <returns></returns>
            private bool DropOneLine()
            {
                int i = 0;
                while (i < RowsCount)
                {
                    if (lineNumbers[i] == 0) break;
                    ++i;
                }
                if (i == RowsCount) return false;

                while (i > 0)
                {
                    lineNumbers[i] = lineNumbers[i - 1];
                    for (int j = 0; j < ColumnsCount; ++j)
                    {
                        cells[j, i] = cells[j, i - 1];
                    }
                    --i;
                }
                lineNumbers[0] = lineNumbers[1] + 1;
                for (int j = 0; j < ColumnsCount; ++j)
                {
                    cells[j, 0] = CellType.None;
                }
                return true;
            }
            /// <summary>
            /// 一段せり上げ
            /// </summary>
            /// <returns></returns>
            private bool LiftUpOneLine()
            {
                int i = RowsCount - 1;
                while (i >= 0)
                {
                    if (lineNumbers[i] == 0) break;
                    --i;
                }
                if (i < 0) return false;
                while (i < RowsCount - 1)
                {
                    lineNumbers[i] = lineNumbers[i + 1];
                    for (int j = 0; j < ColumnsCount; ++j)
                    {
                        cells[j, i] = cells[j, i + 1];
                    }
                    ++i;
                }
                lineNumbers[RowsCount - 1] = lineNumbers[RowsCount - 2] - 1;
                for (int j = 0; j < ColumnsCount; ++j)
                {
                    cells[j, RowsCount - 1] = CellType.None;
                }
                return true;
            }
            /// <summary>
            /// 落下こうげきだまを設定
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="color"></param>
            /// <returns></returns>
            public bool PutDrop(int x, int y, ColorType color)
            {
                CellType cell = (color == ColorType.Red) ? CellType.Red :
                                (color == ColorType.Blue) ? CellType.Blue :
                                (color == ColorType.Green) ? CellType.Green : CellType.Yellow;
                return PutDrop(x, y, cell);
            }
            /// <summary>
            /// 落下こうげきだまを設定
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="color"></param>
            /// <returns></returns>
            public bool PutDrop(int x, int y, CellType color)
            {
                if (x < 0 || x >= ColumnsCount) return false;
                // 初回Drop を置く
                if (lineNumbers[0] == 0)
                {
                    lineNumbers[0] = 1;
                    cells[x, 0] = CellType.DropColumn;
                }
                y += 2;
                while (lineNumbers[0] < y)
                {
                    if (!DropOneLine())
                    {
                        return false;
                    }
                }
                int i = 0;
                while (i < RowsCount && lineNumbers[i] > y)
                {
                    ++i;
                }
                cells[x, i] = color;
                if (i + 1 < RowsCount && lineNumbers[i + 1] == 1)
                {
                    cells[x, i + 1] = CellType.DropColumn;
                }
                return true;
            }
            /// <summary>
            /// せり上げこうげきを設定
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="color"></param>
            /// <returns></returns>
            public bool PutLiftUp(int x, int y, ColorType color)
            {
                CellType cell = (color == ColorType.Red) ? CellType.Red :
                                (color == ColorType.Blue) ? CellType.Blue :
                                (color == ColorType.Green) ? CellType.Green : CellType.Yellow;
                return PutLiftUp(x, y, cell);
            }
            /// <summary>
            /// せり上げこうげきを設定
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="color"></param>
            /// <returns></returns>
            public bool  PutLiftUp(int x, int y, CellType color)
            {
                if (x < 0 || x >= ColumnsCount) return false;
                // 初回LiftUp を置く
                if (lineNumbers[RowsCount - 1] == 0)
                {
                    lineNumbers[RowsCount - 1] = -1;
                    for (int j = 0; j < ColumnsCount; ++j)
                    {
                        cells[j, RowsCount - 1] = CellType.LiftUp;
                    }
                }
                y -= 2;
                while (lineNumbers[RowsCount - 1] > y)
                {
                    if (!LiftUpOneLine())
                    {
                        return false;
                    }
                }
                int i = RowsCount - 1; 
                while (i > 0 && lineNumbers[i] < y)
                {
                    --i;
                }
                cells[x, i] = color;
                return true;
            }
            /// <summary>
            /// 矩形を設定
            /// </summary>
            /// <param name="pixels"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="w"></param>
            /// <param name="h"></param>
            /// <param name="color"></param>
            /// <param name="frame"></param>
            public void SetRect(Color32[] pixels, int x, int y, int w, int h, Color32 color, Color32 frame)
            {
                for (int i = 0; i < w; ++i)
                {
                    for (int j = 0; j < h; ++j)
                    {
                        if (i == 0 || i == w - 1 || j == 0 || j == h - 1)
                        {
                            SetPixel(pixels, x + i, y + j, frame);
                        }
                        else
                        {
                            SetPixel(pixels, x + i, y + j, color);
                        }
                    }
                }
            }
            /// <summary>
            /// セルを指定して矩形を設定
            /// </summary>
            /// <param name="pixels"></param>
            /// <param name="row"></param>
            /// <param name="column"></param>
            /// <param name="color"></param>
            public void SetCellColor(Color32[] pixels, int row, int column, ColorType color)
            {
                int x = Margin + CellSize * column + 1;
                int y = Margin + CellSize * row + 1;
                Color32 c = (color == ColorType.Red) ? red :
                            (color == ColorType.Blue) ? blue : 
                            (color == ColorType.Green) ? green : yellow;
                SetRect(pixels, x, y, CellSize - 2, CellSize - 2, c, black);
            }
            /// <summary>
            /// ピクセルを設定
            /// </summary>
            /// <param name="pixels"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="color"></param>
            private void SetPixel(Color32[] pixels, int x, int y, Color32 color)
            {
                pixels[(Height - 1 - y) * Width + x] = color;
            }
            public enum ArrowType { Up, Down }
            public void SetArrowCell(Color32[] pixels, int row, int column, int rowsCount, int columnsCount, ArrowType arrow)
            {
                int x = Margin + CellSize * column + 1;
                int y = Margin + CellSize * row + 1;
                int width = CellSize * columnsCount - 2;
                int height = (CellSize * rowsCount - 2) / 2;
                int ox = width / 4;
                int w = width - ox * 2;
                if (arrow == ArrowType.Up)
                {
                    SetUpTriangle(pixels, x, y, width, height);
                    SetRect(pixels, x + ox, y + height, w, height, red, red);
                }
                else
                {
                    SetRect(pixels, x + ox, y, w, height, red, red);
                    SetDownTriangle(pixels, x, y + height, width, height);
                }
            }
            /// <summary>
            /// 上向き△
            /// </summary>
            /// <param name="pixels"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public void SetUpTriangle(Color32[] pixels, int x, int y, int width, int height)
            {
                /*
                 * ___xx___
                 * __xxxx__
                 * _xxxxxx_
                 * xxxxxxxx
                 */
                float a = (float)width / 2 / (height - 1);
                for (int i = 0; i < height; ++i)
                {
                    int n = (int)(i * a);
                    int sx = (width / 2) - n;
                    int ex = (width / 2 + 1) + n;
                    for (int j = sx; j < ex; ++j)
                    {
                        if ((x + j) < 0 || (x + j) >= Width) continue;
                        SetPixel(pixels, x + j, y + i, red);
                    }
                }
            }
            /// <summary>
            /// 下向き▽
            /// </summary>
            /// <param name="pixels"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public void SetDownTriangle(Color32[] pixels, int x, int y, int width, int height)
            {
                /*
                 * xxxxxxxx
                 * _xxxxxx_
                 * __xxxx__
                 * ___xx___
                 */
                y += (height - 1);
                float a = (float)width / 2 / (height - 1);
                for (int i = 0; i < height; ++i)
                {
                    int n = (int)(i * a);
                    int sx = (width / 2) - n;
                    int ex = (width / 2 + 1) + n;
                    for (int j = sx; j < ex; ++j)
                    {
                        if ((x + j) < 0 || (x + j) >= Width) continue;
                        SetPixel(pixels, x + j, y - i, red);
                    }
                }
            }

            /// <summary>
            /// 配置をピクセルに反映させる
            /// </summary>
            /// <param name="pixels"></param>
            public void Apply(Color32[] pixels)
            {
                for (int i = 0; i < RowsCount; ++i)
                {
                    for (int j = 0; j < ColumnsCount; ++j)
                    {
                        if (cells[j, i] == CellType.None) continue;
                        switch (cells[j, i])
                        {
                            case CellType.Red:
                                SetCellColor(pixels, i, j, ColorType.Red);
                                break;
                            case CellType.Blue:
                                SetCellColor(pixels, i, j, ColorType.Blue);
                                break;
                            case CellType.Green:
                                SetCellColor(pixels, i, j, ColorType.Green);
                                break;
                            case CellType.Yellow:
                                SetCellColor(pixels, i, j, ColorType.Yellow);
                                break;
                            case CellType.DropColumn:
                                SetArrowCell(pixels, i, j, 1, 1, ArrowType.Down);
                                break;
                            case CellType.DropRow:
                                if (j == 0 || cells[j - 1, i] != CellType.DropRow)
                                {
                                    int n = 1;
                                    while ((j + n < ColumnsCount) && (cells[j + n, i] == CellType.DropRow))
                                    {
                                        ++n;
                                    }
                                    SetArrowCell(pixels, i, j, n, 1, ArrowType.Down);
                                }
                                break;
                            case CellType.LiftUp:
                                SetArrowCell(pixels, i, j, 1, 1, ArrowType.Up);
                                break;
                        }
                    }
                }
            }
            /// <summary>
            /// 下向き矢印を連結
            /// </summary>
            /// <param name="current"></param>
            /// <param name="last"></param>
            public void JointDropArrow(AttackPatternData.AttackTamaData current, AttackPatternData.AttackTamaData last)
            {
                if (current == null || last == null) return;
                // 最下段のみ対応
                if (current.Position.y != 0 || last.Position.y != 0) return;
                if (Mathf.Abs(current.Position.x - last.Position.x) != 1) return;
                int i = 0;
                while (i < lineNumbers.Length)
                {
                    if (lineNumbers[i] == -1)
                    {
                        cells[current.Position.x, i] = CellType.DropRow;
                        cells[last.Position.x, i] = CellType.DropRow;
                        break;
                    }
                    ++i;
                }
            }

        }
        /// <summary>
        /// テクスチャ生成
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static Texture2D Generate(AttackPatternData data, int width, int height, IRowColInfo setting)
        {
            Context context = new Context(width, height, setting.ColumnsCount);

            Color32[] pixels = new Color32[width * height];
            Color32 black = new Color32(0, 0, 0, 255);
            Color32 white = new Color32(255, 255, 255, 255);
            context.SetRect(pixels, 0, 0, width, height, white, black);
            int dropIndex = 0;
            int liftUpIndex = 0;

            int leftSampleCount = (data.SampleCount == 0) ? 24 : data.SampleCount;

            for (int i = 0; (i < data.AttackDirectionPattern.Length) && (leftSampleCount > 0); ++i)
            {
                int count = System.Math.Min(leftSampleCount, 6);
                leftSampleCount -= count;

                if (data.AttackDirectionPattern[i] == AttackPatternData.AttackDirectionType.Top)
                {
                    for (int j = 0; j < count; ++j)
                    {
                        bool set = false;
                        do
                        {
                            var a = data.DropAttackTamaPattern[dropIndex++];
                            if (a.Position.y >= data.SampleSkipRows)
                            {
                                context.PutDrop(a.Position.x, a.Position.y - data.SampleSkipRows, a.Color);
                                set = true;
                            }
                        } while (!set);
                    }
                }
                else
                {
                    for (int j = 0; j < count; ++j)
                    {
                        var a = data.LiftAttackTamaPattern[liftUpIndex++];
                        if (liftUpIndex == data.LiftAttackTamaPattern.Length)
                        {
                            liftUpIndex = 0;
                        }
                        context.PutLiftUp(a.Position.x, -a.Position.y, a.Color);
                    }
                }
            }
            context.Apply(pixels);

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }
    }
}
