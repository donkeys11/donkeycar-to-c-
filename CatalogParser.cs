using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DonkeycarManager
{
    public class CatalogParser
    {
        // catalog 파일 읽어서 프레임 리스트로 반환
        public static List<DonkeyFrame> Load(string catalogFilePath)
        {
            var frames = new List<DonkeyFrame>();

            if (!File.Exists(catalogFilePath))
                throw new FileNotFoundException("catalog 파일을 찾을 수 없습니다.", catalogFilePath);

            foreach (string line in File.ReadLines(catalogFilePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    DonkeyFrame? frame = JsonSerializer.Deserialize<DonkeyFrame>(line);

                    if (frame != null && !string.IsNullOrWhiteSpace(frame.ImageFileName))
                        frames.Add(frame);
                }
                catch (JsonException)
                {
                    Console.WriteLine($"[파싱 실패] {line}");
                }
            }

            return frames;
        }
        
        // data 폴더 안의 catalog 파일을 전부 읽어서 합쳐서 반환
        public static List<DonkeyFrame> LoadAll(string dataFolderPath)
        {
            var allFrames = new List<DonkeyFrame>();

            // catalog_0, catalog_1, catalog_2 ... 순서대로 탐색
            for (int i = 0; i < 100; i++)
            {
                string path = Path.Combine(dataFolderPath, $"catalog_{i}.catalog");

                // 파일이 없으면 탐색 중단
                if (!File.Exists(path))
                    break;

                List<DonkeyFrame> frames = Load(path);
                allFrames.AddRange(frames);
            }

            if (allFrames.Count == 0)
                throw new FileNotFoundException("catalog 파일을 찾을 수 없습니다.");

            return allFrames;
        }
       
        // 프레임 리스트를 catalog 파일로 저장
        public static void Save(string catalogFilePath, List<DonkeyFrame> frames)
        {
            var lines = new List<string>();

            foreach (DonkeyFrame frame in frames)
            {
                var obj = new Dictionary<string, object?>
                {
                    ["_index"] = frame.Index,
                    ["_session_id"] = frame.SessionId,
                    ["_timestamp_ms"] = frame.TimestampMs,
                    ["cam/image_array"] = frame.ImageFileName,
                    ["user/angle"] = frame.Angle,
                    ["user/mode"] = frame.Mode,
                    ["user/throttle"] = frame.Throttle
                };

                lines.Add(JsonSerializer.Serialize(obj));
            }

            File.WriteAllLines(catalogFilePath, lines);
        }
    }
}