using System;
using System.Collections.Generic;
using Domain.Grid;
using UnityEngine;

namespace Services.Map {
    public class MapParser {
        public HexGridData Parse(TextAsset textAsset) {
            if (!textAsset)
                throw new ArgumentNullException(nameof(textAsset));

            var lines = ParseTextIntoLines(textAsset.text ?? string.Empty);
            var width = lines[0].Length;
            var height = lines.Count;
            if (width == 0)
                throw new FormatException("Map width is zero.");

            return CreateHexGridData(lines, width, height);
        }

        static List<string> ParseTextIntoLines(string text) {
            var lines = new List<string>();
            var lineStart = 0;

            for (var i = 0; i <= text.Length; i++) {
                var isLineEnd = i == text.Length || text[i] == '\n';
                if (!isLineEnd)
                    continue;

                if (i - lineStart > 0) {
                    var lineEndIndex = (i > 0 && text[i - 1] == '\r') ? i - 1 : i;
                    var line = text.Substring(lineStart, lineEndIndex - lineStart).Trim();
                    if (line.Length > 0)
                        lines.Add(line);
                }

                lineStart = i + 1;
            }

            if (lines.Count == 0)
                throw new FormatException("Map file is empty.");

            return lines;
        }

        static HexGridData CreateHexGridData(List<string> lines, int width, int height) {
            var tiles = new HexTileType[width * height];

            for (var row = 0; row < height; row++) {
                var line = lines[row];
                ValidateLineWidth(line, width, row);

                for (var col = 0; col < width; col++) {
                    tiles[row * width + col] = ParseCharToTileType(line[col]);
                }
            }

            return new HexGridData(width, height, tiles);
        }

        static void ValidateLineWidth(string line, int expectedWidth, int rowIndex) {
            if (line.Length != expectedWidth)
                throw new FormatException(
                    $"Non-rectangular map. Row {rowIndex} has {line.Length} columns; expected {expectedWidth}.");
        }
        
        static HexTileType ParseCharToTileType(char c) => c switch {
            '0' => HexTileType.Water,
            '1' => HexTileType.Terrain,
            _ => throw new FormatException($"Invalid tile type: {c}")
        };
    }
}