using System;
using System.Collections.Generic;
using System.Linq;

namespace DonkeycarManager
{
    // 데이터 품질 요약 정보를 담는 클래스
    public class DataSummary
    {
        public int TotalFrames { get; set; }        // 전체 프레임 수
        public int StopFrames { get; set; }         // 정지 프레임 수 (throttle == 0)
        public int ZeroAngleFrames { get; set; }    // 직진 프레임 수 (angle == 0)
        public double AverageAngle { get; set; }    // 평균 핸들 각도
        public double AverageThrottle { get; set; } // 평균 속도
        public double StopRatio { get; set; }       // 정지 비율 (%)

        // 프레임 리스트를 받아서 요약 정보 계산
        public static DataSummary Calculate(List<DonkeyFrame> frames)
        {
            if (frames == null || frames.Count == 0)
                return new DataSummary();

            int total = frames.Count;
            int stopFrames = frames.Count(f => Math.Abs(f.Throttle) <= 0.000001);
            int zeroAngle = frames.Count(f => Math.Abs(f.Angle) <= 0.000001);
            double avgAngle = frames.Average(f => f.Angle);
            double avgThrottle = frames.Average(f => f.Throttle);
            double stopRatio = (double)stopFrames / total * 100;

            return new DataSummary
            {
                TotalFrames = total,
                StopFrames = stopFrames,
                ZeroAngleFrames = zeroAngle,
                AverageAngle = avgAngle,
                AverageThrottle = avgThrottle,
                StopRatio = stopRatio
            };
        }

        // 요약 정보를 문자열로 반환
        public override string ToString()
        {
            return
                $"전체 프레임 수: {TotalFrames}개\n" +
                $"정지 프레임 수: {StopFrames}개 ({StopRatio:F1}%)\n" +
                $"직진 프레임 수: {ZeroAngleFrames}개\n" +
                $"평균 핸들 각도: {AverageAngle:F4}\n" +
                $"평균 속도: {AverageThrottle:F4}";
        }
    }
}