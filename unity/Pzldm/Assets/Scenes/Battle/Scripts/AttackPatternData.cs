using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pzldm
{
    /// <summary>
    /// こうげきだまパターンデータ
    /// </summary>
    [CreateAssetMenu(menuName = "Pzldm/AttackPatternData")]
    public class AttackPatternData : ScriptableObject
    {
        /// <summary>
        /// こうげきだまの方向
        /// </summary>
        public enum AttackDirectionType
        {
            /// <summary>
            /// 上から落下
            /// </summary>
            Top,
            /// <summary>
            /// 下からせり上がり
            /// </summary>
            Bottom,
        }

        /// <summary>
        /// こうげきだまの配置データ
        /// </summary>
        [System.Serializable]
        public class AttackTamaData
        {
            public Vector2Int Position;
            public ColorType Color;
        }
        /// <summary>
        /// キャラクター名
        /// </summary>
        [SerializeField]
        private string characterName;
        /// <summary>
        /// キャラクター名
        /// </summary>
        public string CharacterName
        {
            get { return characterName; }
            set { characterName = value; }
        }
        /// <summary>
        /// こうげきだま方向の順序
        /// </summary>
        [SerializeField]
        private AttackDirectionType[] attackDirectionPattern;
        /// <summary>
        /// こうげきだま方向の順序
        /// </summary>
        public AttackDirectionType[] AttackDirectionPattern { get { return attackDirectionPattern; } }
        /// <summary>
        /// こうげきだまの１セットのうち落下分の最大数
        /// </summary>
        public int MaxDropLines
        {
            get
            {
                int count = 0;
                if (AttackDirectionPattern != null)
                {
                    foreach (var d in AttackDirectionPattern)
                    {
                        if (d == AttackDirectionType.Top) ++count;
                    }
                }
                // 最低でも１段は降る
                return System.Math.Max(1, count);
            }
        }
        /// <summary>
        /// せり上げこうげきパターンを持っているかどうか
        /// </summary>
        public bool HasLifting
        {
            get
            {
                if ((AttackDirectionPattern == null) || (AttackDirectionPattern.Length == 0)) return false;
                foreach (var dir in AttackDirectionPattern)
                {
                    if (dir == AttackDirectionType.Bottom)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// 上からのこうげきだまのパターン(落下順)
        /// </summary>
        [SerializeField]
        private AttackTamaData[] dropAttackTamaPattern;
        /// <summary>
        /// 上からのこうげきだまのパターン(落下順)
        /// </summary>
        public AttackTamaData[] DropAttackTamaPattern { get { return dropAttackTamaPattern; } }
        /// <summary>
        /// 下からのこうげきだまのパターン(せり上がり順)
        /// </summary>
        [SerializeField]
        private AttackTamaData[] liftAttackTamaPattern;
        /// <summary>
        /// 下からのこうげきだまのパターン(せり上がり順)
        /// </summary>
        public AttackTamaData[] LiftAttackTamaPattern { get { return liftAttackTamaPattern; } }

        [SerializeField]
        private int sampleCount;
        /// <summary>
        /// サンプル出力時にいくつまで参照するか
        /// </summary>
        public int SampleCount
        {
            get { return sampleCount; }
            set { sampleCount = value; }
        }
        [SerializeField]
        private int sampleSkipRows;
        /// <summary>
        /// サンプル出力時に省略する行(段)
        /// </summary>
        public int SampleSkipRows
        {
            get { return sampleSkipRows; }
            set { sampleSkipRows = value; }
        }

        /// <summary>
        /// 色の順序
        /// </summary>
        public enum ColorOrderType
        {
            /// <summary>
            /// 赤青緑黄
            /// </summary>
            RBGY,
            /// <summary>
            /// 赤青黄緑
            /// </summary>
            RBYG,
            /// <summary>
            /// その他
            /// </summary>
            Other
        }
        /// <summary>
        /// 落下順序プリセット
        /// </summary>
        public enum DropOrderPresetType
        {
            /// <summary>
            /// 左下から横優先
            /// </summary>
            LeftBottomStart,
            /// <summary>
            /// 中央から縦優先
            /// </summary>
            CenterStart,
            /// <summary>
            /// 両端から縦優先
            /// </summary>
            BothSideStart,
            /// <summary>
            /// 奇数(0開始)列、右端から１列おき
            /// </summary>
            OddColumnRightStart,
            /// <summary>
            /// 縦横交互
            /// </summary>
            VerticalHorizontalAlternately,
            /// <summary>
            /// L字型
            /// </summary>
            LCharacter,
            /// <summary>
            /// 逆L字型
            /// </summary>
            ReversedLCharacter,
            /// <summary>
            /// 左から順に１段ずつ
            /// </summary>
            FlatLeftStart,
        }
        /// <summary>
        /// 色の並び方向
        /// </summary>
        public enum ColorLineType
        {
            /// <summary>
            /// 縦
            /// </summary>
            Vertical,
            /// <summary>
            /// 横
            /// </summary>
            Horizontal,
            /// <summary>
            /// 斜め
            /// </summary>
            Diagonal,
            /// <summary>
            /// 山型
            /// </summary>
            Mountain,
            /// <summary>
            /// 平行四辺形落下
            /// </summary>
            ParallelogramDrop,
            /// <summary>
            /// 縦横交互パターン
            /// </summary>
            VerticalHorizontalAlternately,
            /// <summary>
            /// L字型パターン
            /// </summary>
            LCharacter,
            /// <summary>
            /// 逆L字型パターン
            /// </summary>
            ReverseLCharacter,
        }
        /// <summary>
        /// せり上げのカラーパターン
        /// </summary>
        public enum LiftingColorLineType
        {
            /// <summary>
            /// せり上げ無し
            /// </summary>
            None,
            /// <summary>
            /// 縦
            /// </summary>
            Vertical,
            /// <summary>
            /// 横
            /// </summary>
            Horizontal,
            /// <summary>
            /// 縦横せり上げ
            /// </summary>
            VerticalHorizontalAlternately,
        }
        /// <summary>
        /// パターン部分の上の落下順序を埋める
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="columns"></param>
        /// <param name="patternRows"></param>
        private static void GenerateDropOrderOverPatternRows(AttackTamaData[] datas, int columns, int patternRows)
        {
            int x = 0;
            int y = patternRows;
            int index = columns * patternRows;
            while (index < datas.Length)
            {
                x = 0;
                while ((x < columns) && (index < datas.Length))
                {
                    datas[index] = new AttackTamaData() { Position = new Vector2Int(x, y) };
                    ++index;
                    ++x;
                }
                ++y;
            }
        }
        /// <summary>
        /// 左下から斜めに降るパターンの順序を生成
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        /// <returns></returns>
        public static AttackTamaData[] GenerateDropOrderLeftDownStart(int columns, int rows, int patternRows)
        {
            AttackTamaData[] datas = new AttackTamaData[columns * rows];
            int index = 0;
            // 最初の斜め
            int x = 0;
            int y = 0;
            int n = 0;
            while (n < columns)
            {
                datas[index] = new AttackTamaData() { Position = new Vector2Int(x, y) };
                ++index;
                if (x == 0)
                {
                    ++n;
                    x = n;
                    y = 0;
                }
                else
                {
                    --x;
                    ++y;
                }
            }
            // パターン最上段まで
            x = columns - 1;
            y = 1;
            n = 1;
            while (y < patternRows)
            {
                datas[index] = new AttackTamaData() { Position = new Vector2Int(x, y) };
                ++index;
                if ((x == 0) || (y == patternRows - 1))
                {
                    if (n == patternRows - 1)
                    {
                        break;
                    }
                    x = columns - 1;
                    ++n;
                    y = n;
                }
                else
                {
                    --x;
                    ++y;
                }
            }
            // パターン最上段以降
            GenerateDropOrderOverPatternRows(datas, columns, patternRows);
            return datas;
        }
        /// <summary>
        /// 両端優先パターン
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        /// <returns></returns>
        public static AttackTamaData[] GenerateDropOrderBothSideStart(int columns, int rows, int patternRows)
        {
            AttackTamaData[] datas = new AttackTamaData[columns * rows];
            int col = columns / 2;
            int x = 0;
            int y = 0;
            int index = 0;
            // 最初の斜め
            int n = 1;
            while (n < col)
            {
                datas[index] = new AttackTamaData() { Position = new Vector2Int(x, y) };
                ++index;
                datas[index] = new AttackTamaData() { Position = new Vector2Int(columns - 1 - x, y) };
                ++index;
                if (y == 0)
                {
                    y = n;
                    x = 0;
                    ++n;
                }
                else
                {
                    ++x;
                    --y;
                }
            }
            // パターン最上段まで
            while (n < patternRows + col)
            {
                datas[index] = new AttackTamaData() { Position = new Vector2Int(x, y) };
                ++index;
                datas[index] = new AttackTamaData() { Position = new Vector2Int(columns - 1 - x, y) };
                ++index;
                
                ++x;
                if (x == col)
                {
                    if (n >= patternRows)
                    {
                        x = n - patternRows + 1;
                        y = patternRows - 1;
                    }
                    else
                    {
                        x = 0;
                        y = n;
                    }
                    ++n;
                }
                else
                {
                    --y;
                }
            }
            // パターン最上段以降
            GenerateDropOrderOverPatternRows(datas, columns, patternRows);
            return datas;
        }
        /// <summary>
        /// 真ん中優先
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        /// <returns></returns>
        public static AttackTamaData[] GenerateDropOrderCenterStart(int columns, int rows, int patternRows)
        {
            var datas = GenerateDropOrderBothSideStart(columns, rows, patternRows);
            // パターン部分の反転
            int count = columns * patternRows;
            int col = columns / 2;
            for (int i = 0; i < count; ++i)
            {
                var pos = datas[i].Position;
                if (pos.x < col)
                {
                    datas[i].Position = new Vector2Int(col - 1 - pos.x, pos.y);
                }
                else
                {
                    datas[i].Position = new Vector2Int(columns - 1 - pos.x + col, pos.y);
                }
            }
            return datas;
        }
        /// <summary>
        /// 0開始の奇数列右から
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        /// <returns></returns>
        public static AttackTamaData[] GenerateDropOrderOddColumnRightStart(int columns, int rows, int patternRows)
        {
            AttackTamaData[] datas = new AttackTamaData[columns * rows];
            int col = columns / 2;
            int count = columns * patternRows;
            int index = 0;
            for (int i = 0; i < columns; ++i)
            {
                int x = (columns - 1 - ((i % col) * 2));
                if (i >= col)
                {
                    --x;
                }

                for (int y = 0; y < patternRows; ++y)
                {
                    datas[index] = new AttackTamaData() { Position = new Vector2Int(x, y) };
                    ++index;
                }
            }
            // パターン最上段以降
            GenerateDropOrderOverPatternRows(datas, columns, patternRows);
            return datas; 
        }
        /// <summary>
        /// １段ずつ左から
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        /// <returns></returns>
        public static AttackTamaData[] GenerateDropOrderFlatLeftStart(int columns, int rows, int patternRows)
        {
            AttackTamaData[] datas = new AttackTamaData[columns * rows];
            GenerateDropOrderOverPatternRows(datas, columns, 0);
            return datas;
        }
        /// <summary>
        /// 縦横交互
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        /// <returns></returns>
        public static AttackTamaData[] GenerateDropOrderVerticalHorizontalAlternately(int columns, int rows, int patternRows)
        {
            AttackTamaData[] datas = new AttackTamaData[columns * rows];

            int col = columns / 2;
            // パターンの段数をcol の倍数になるよう補正
            patternRows = patternRows / col * col;
            // ブロックの座標更新
            System.Func<int, int, bool, (int a, int b)> ProgressPos = (int x, int y, bool hor) =>
            {
                if (hor)
                {
                    ++x;
                    if (x == col)
                    {
                        x = 0;
                        ++y;
                    }
                }
                else
                {
                    ++y;
                    if (y == col)
                    {
                        y = 0;
                        ++x;
                    }
                }
                return (x, y);
            };
            int blockCount = col * col;
            // パターン部分を埋めていく
            int index = 0;
            bool horizontal = false;
            bool reverse = false;
            int xOffset = 0;
            int yOffset = 0;
            while (yOffset < patternRows)
            {
                int blockIndex = 0;
                int x = 0;
                int y = 0;
                while (blockIndex < blockCount)
                {
                    var pos = new Vector2Int(xOffset + x, yOffset + y);
                    if (reverse)
                    {
                        // 左右反転
                        pos.x = columns - pos.x - 1;
                    }
                    datas[index] = new AttackTamaData() { Position = pos };
                    (x, y) = ProgressPos(x, y, horizontal);
                    ++index;
                    ++blockIndex;
                }
                xOffset += col;
                horizontal = !horizontal;
                if (xOffset >= columns)
                {
                    xOffset = 0;
                    yOffset += col;
                    reverse = !reverse;
                }
            }
            // 残り
            GenerateDropOrderOverPatternRows(datas, columns, patternRows);

            return datas;
        }
        /// <summary>
        /// L字型
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        /// <returns></returns>
        public static AttackTamaData[] GenerateDropOrderLCharacter(int columns, int rows, int patternRows)
        {
            AttackTamaData[] datas = new AttackTamaData[columns * rows];
            int index = 0;
            int n = 0;
            while (n < columns)
            {
                int x = columns - 1;
                int y = n;
                while (x > n)
                {
                    datas[index] = new AttackTamaData() { Position = new Vector2Int(x, y) };
                    ++index;
                    --x;
                }
                ++n;
                while (y < patternRows)
                {
                    datas[index] = new AttackTamaData() { Position = new Vector2Int(x, y) };
                    ++index;
                    ++y;
                }
            }
            // 残り
            GenerateDropOrderOverPatternRows(datas, columns, patternRows);

            return datas;
        }
        /// <summary>
        /// 逆L字
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        /// <returns></returns>
        public static AttackTamaData[] GenerateDropOrderReverseLCharacter(int columns, int rows, int patternRows)
        {
            AttackTamaData[] datas = new AttackTamaData[columns * rows];
            int y = patternRows - 1;
            int x = columns - 1;
            int n = 0;
            int index = columns * patternRows - 1;
            while (index >= 0)
            {
                datas[index] = new AttackTamaData() { Position = new Vector2Int(x, y) };
                --index;
                if (y == 0)
                {
                    ++n;
                    y = patternRows - 1 - n;
                    x = columns - 1;
                }
                else if (x == n)
                {
                    --y;
                }
                else
                {
                    --x;
                }
            }
            // 残り
            GenerateDropOrderOverPatternRows(datas, columns, patternRows);

            return datas;
        }
        /// <summary>
        /// たま落下順序パターン生成
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        /// <returns></returns>
        public static AttackTamaData[] GenerateDropOrder(DropOrderPresetType preset, int columns, int rows, int patternRows)
        {
            System.Func<int, int, int, AttackTamaData[]> generator;
            switch (preset)
            {
                case DropOrderPresetType.LeftBottomStart:
                    generator = GenerateDropOrderLeftDownStart;
                    break;
                case DropOrderPresetType.CenterStart:
                    generator = GenerateDropOrderCenterStart;
                    break;
                case DropOrderPresetType.BothSideStart:
                    generator = GenerateDropOrderBothSideStart;
                    break;
                case DropOrderPresetType.OddColumnRightStart:
                    generator = GenerateDropOrderOddColumnRightStart;
                    break;
                case DropOrderPresetType.VerticalHorizontalAlternately:
                    generator = GenerateDropOrderVerticalHorizontalAlternately;
                    break;
                case DropOrderPresetType.LCharacter:
                    generator = GenerateDropOrderLCharacter;
                    break;
                case DropOrderPresetType.ReversedLCharacter:
                    generator = GenerateDropOrderReverseLCharacter;
                    break;
                default:
                    generator = GenerateDropOrderFlatLeftStart;
                    break;
            }
            return generator(columns, rows, patternRows);
        }
        /// <summary>
        /// タイプからカラーパターン生成
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private static IEnumerator<ColorType> GenerateColorPattern(ColorOrderType order)
        {
            if (order == ColorOrderType.RBYG)
            {
                while (true)
                {
                    yield return ColorType.Red;
                    yield return ColorType.Blue;
                    yield return ColorType.Yellow;
                    yield return ColorType.Green;
                }
            }
            else
            {
                while (true)
                {
                    yield return ColorType.Red;
                    yield return ColorType.Blue;
                    yield return ColorType.Green;
                    yield return ColorType.Yellow;
                }
            }
        }
        private static ColorType[] GenerateColorPattern(ColorOrderType order, int columns)
        {
            ColorType[] pattern = new ColorType[columns];
            var generator = GenerateColorPattern(order);
            for (int i = 0; i < columns; ++i)
            {
                generator.MoveNext();
                pattern[i] = generator.Current;
            }
            return pattern;
        }
        /// <summary>
        /// 配列からパターン生成
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static ColorType[] GenerateColorPattern(ColorType[] source, int columns)
        {
            ColorType[] pattern = new ColorType[columns];
            int index = 0;
            for (int i = 0; i < columns; ++i)
            {
                pattern[i] = source[index];
                ++index;
                if (index == source.Length)
                {
                    index = 0;
                }
            }
            return pattern;
        }

        /// <summary>
        /// 縦型パターンを生成
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public static void GenerateColorPatternVertical(AttackTamaData[] datas, ColorOrderType order, int columns, int rows, int patternRows)
        {
            var pattern = GenerateColorPattern(order, columns);
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                data.Color = pattern[data.Position.x];
            }
        }
        /// <summary>
        /// 横型パターンを生成
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public static void GenerateColorPatternHorizontal(AttackTamaData[] datas, ColorOrderType order, int columns, int rows, int patternRows)
        {
            var pattern = GenerateColorPattern(order, rows);
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                data.Color = pattern[data.Position.y];
            }
        }
        /// <summary>
        /// ななめパターンを生成
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public static void GenerateColorPatternDiagonal(AttackTamaData[] datas, ColorOrderType order, int columns, int rows, int patternRows)
        {
            var pattern = GenerateColorPattern(order, rows + columns);
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                data.Color = pattern[data.Position.y + (columns - data.Position.x - 1)];
            }
        }
        /// <summary>
        /// 山型パターンを生成
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public static void GenerateColorPatternMountain(AttackTamaData[] datas, ColorOrderType order, int columns, int rows, int patternRows)
        {
            var col = columns / 2;
            var pattern = GenerateColorPattern(order, rows + col);
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                int offset = data.Position.x;
                if (offset >= col)
                {
                    offset = columns - 1 - offset;
                }
                offset = col - offset - 1;
                data.Color = pattern[data.Position.y + offset];
            }
        }
        /// <summary>
        /// 縦横交互パターン
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public static void GenerateColorPatternVerticalHorizontalAlternately(AttackTamaData[] datas, ColorOrderType order, int columns, int rows, int patternRows)
        {
            int col = columns / 2;
            // パターン部分をcol の倍数に補正
            patternRows = patternRows / col * col; 
            // パターン部分縦
            ColorType[] vPattern = GenerateColorPattern(new ColorType[] { ColorType.Green, ColorType.Yellow, ColorType.Blue, ColorType.Red }, columns);
            // パターン部分横
            ColorType[] hPattern = GenerateColorPattern(new ColorType[] { ColorType.Red, ColorType.Green, ColorType.Yellow, ColorType.Blue }, patternRows);
            // パターン部分より上
            ColorType[] oPattern = GenerateColorPattern(new ColorType[] { ColorType.Blue, ColorType.Red, ColorType.Green, ColorType.Yellow }, rows - patternRows);
            int count = columns * patternRows;
            int index = 0;
            while (index < count)
            {
                var data = datas[index];
                // 縦横の奇数ブロックかどうか
                bool vOdd = ((data.Position.y / 3) & 1) != 0;
                bool hOdd = ((data.Position.x / 3) & 1) != 0;
                // 横パターンブロックかどうか
                bool hor = vOdd ^ hOdd;
                if (hor)
                {
                    data.Color = hPattern[data.Position.y];
                }
                else
                {
                    data.Color = vPattern[data.Position.x];
                }
                ++index;
            }
            // パターン部分より上
            while (index < datas.Length)
            {
                var data = datas[index];
                data.Color = oPattern[data.Position.y - patternRows];
                ++index;
            }
        }
        /// <summary>
        /// L字パターン
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public static void GenerateColorPatternLCharacter(AttackTamaData[] datas, ColorOrderType order, int columns, int rows, int patternRows)
        {
            var pattern = GenerateColorPattern(order, columns);
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                int index = System.Math.Min(data.Position.x, data.Position.y);
                data.Color = pattern[index];
            }
        }
        /// <summary>
        /// 逆L字パターン
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public static void GenerateColorPatternReverseLCharacter(AttackTamaData[] datas, ColorOrderType order, int columns, int rows, int patternRows)
        {
            var pattern = GenerateColorPattern(order, columns);
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                int y = (patternRows - 1 - data.Position.y);
                var index = y;
                while (index < 0) index += 4;
                index %= 4;
                index = (data.Position.x < y) ? data.Position.x : index;
                data.Color = pattern[index];
            }
        }
        /// <summary>
        /// ひし形落下パターン
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public static void GenerateColorPatternParallelogram(AttackTamaData[] datas, ColorOrderType order, int columns, int rows, int patternRows)
        {
            var pattern = GenerateColorPattern(order, columns);
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                int y = data.Position.y % columns;
                int x = columns - 1 - data.Position.x;
                if (x < y)
                {
                    y -= columns;
                    while (y < 0) y += 4;
                }
                data.Color = pattern[y];
            }
        }
        /// <summary>
        /// 縦横交互のせり上げ
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public static void GenerateColorPatternVerticalHorizontalAlternatelyLifting(AttackTamaData[] datas, ColorOrderType order, int columns, int rows, int patternRows)
        {
            var pattern = GenerateColorPattern(order, columns);
            for (int i = 0; i < datas.Length; ++i)
            {
                var data = datas[i];
                var index = data.Position.y / 3;
                bool odd = (index & 1) != 0;
                if (odd)
                {
                    int r = 3 * (data.Position.y / 6) + (data.Position.y % 6) - 3;
                    r %= 4;
                    data.Color = pattern[r];
                }
                else
                {
                    data.Color = pattern[data.Position.x];
                }
            }
        }
        /// <summary>
        /// 座標が設定されているデータに色を設定
        /// </summary>
        /// <param name="datas">座標が設定されているデータ配列</param>
        /// <param name="preset">落下順序プリセット</param>
        /// <param name="line">配色パターン</param>
        /// <param name="order">色の順序</param>
        /// <param name="columns">列数</param>
        /// <param name="rows">行数</param>
        /// <param name="patternRows">パターンの行数</param>
        public static void GenerateColorPattern(AttackTamaData[] datas, ColorLineType line, ColorOrderType order, int columns, int rows, int patternRows)
        {
            System.Action<AttackTamaData[], ColorOrderType, int, int, int> generator;
            switch (line)
            {
                case ColorLineType.Vertical:
                    generator = GenerateColorPatternVertical;
                    break;
                case ColorLineType.Horizontal:
                    generator = GenerateColorPatternHorizontal;
                    break;
                case ColorLineType.Diagonal:
                    generator = GenerateColorPatternDiagonal;
                    break;
                case ColorLineType.Mountain:
                    generator = GenerateColorPatternMountain;
                    break;
                case ColorLineType.ParallelogramDrop:
                    generator = GenerateColorPatternParallelogram;
                    break;
                case ColorLineType.VerticalHorizontalAlternately:
                    generator = GenerateColorPatternVerticalHorizontalAlternately;
                    break;
                case ColorLineType.LCharacter:
                    generator = GenerateColorPatternLCharacter;
                    break;
                case ColorLineType.ReverseLCharacter:
                    generator = GenerateColorPatternReverseLCharacter;
                    break;
                default:
                    generator = (d, l, c, r, p) => { for (int i = 0; i < d.Length; ++i) d[i].Color = ColorType.Yellow; };
                    break;
            }
            generator(datas, order, columns, rows, patternRows);
        }
        /// <summary>
        /// プリセットを指定してこうげきだまパターンを生成
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="line"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="patternRows"></param>
        public void SetupDropPreset(DropOrderPresetType preset, ColorLineType line, ColorOrderType order, int columns, int rows, int patternRows)
        {
            var datas = GenerateDropOrder(preset, columns, rows, patternRows);
            GenerateColorPattern(datas, line, order, columns, rows, patternRows);
            dropAttackTamaPattern = datas;
        }
        /// <summary>
        /// プリセットを指定してせり上げのこうげきだまパターンを生成
        /// </summary>
        /// <param name="line"></param>
        /// <param name="order"></param>
        /// <param name="columns"></param>
        public void SetupLiftingPreset(LiftingColorLineType line, ColorOrderType order, int columns)
        {
            switch (line)
            {
                case LiftingColorLineType.None:
                    liftAttackTamaPattern = null;
                    break;
                case LiftingColorLineType.Horizontal:
                    {
                        var datas = GenerateDropOrderFlatLeftStart(columns, 4, 0);
                        GenerateColorPatternHorizontal(datas, order, columns, 4, 0);
                        liftAttackTamaPattern = datas;
                    }
                    break;
                case LiftingColorLineType.Vertical:
                    {
                        var datas = GenerateDropOrderFlatLeftStart(columns, 1, 0);
                        GenerateColorPatternVertical(datas, order, columns, 1, 0);
                        liftAttackTamaPattern = datas;
                    }
                    break;
                case LiftingColorLineType.VerticalHorizontalAlternately:
                    {
                        var datas = GenerateDropOrderFlatLeftStart(columns, 24, 0);
                        GenerateColorPatternVerticalHorizontalAlternatelyLifting(datas, order, columns, 24, 0);
                        liftAttackTamaPattern = datas;
                    }
                    break;
            }
        }
        /// <summary>
        /// プリセットから設定
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="setting"></param>
        public void SetupWithPreset(AttackPatternPreset.Preset preset, int columnsCount, int rowsCount, int patternRowsCount)
        {
            CharacterName = preset.CharacterName;
            SetupDropPreset(preset.DropOrder, preset.ColorLine, preset.ColorOrder, columnsCount, rowsCount, patternRowsCount);
            SetupLiftingPreset(preset.LiftingColorLine, preset.ColorOrder, columnsCount);
            attackDirectionPattern = new AttackDirectionType[preset.DirectionPattern.Length];
            for (int i = 0; i < attackDirectionPattern.Length; ++i)
            {
                attackDirectionPattern[i] = preset.DirectionPattern[i];
            }
        }
    }
}
