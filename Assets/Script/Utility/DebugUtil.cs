using UnityEngine;

// 디버그 메시지의 색상을 정의하는 열거형
public enum DebugColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Cyan,
    Magenta,
    Default,
    Black
}

public class DebugUtil
{
    // DebugColor 열거형을 UnityEngine.Color로 변환하는 함수
    private static Color GetColor(DebugColor color)
    {
        switch (color)
        {
            case DebugColor.Red:
                return Color.red;
            case DebugColor.Green:
                return Color.green;
            case DebugColor.Blue:
                return Color.blue;
            case DebugColor.Yellow:
                return Color.yellow;
            case DebugColor.Cyan:
                return Color.cyan;
            case DebugColor.Magenta:
                return Color.magenta;
            case DebugColor.Default:
                return Color.white;
            case DebugColor.Black:
                return Color.black;
            default:
                return Color.white;
        }
    }

    // 로그 활성화 여부를 저장하는 변수
    private static bool isLoggingEnabled = IsLoggingActive();

    // 로그 활성화 여부를 결정하는 함수, 개발 빌드나 에디터에서만 활성화되도록 설정
    private static bool IsLoggingActive()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        return true;
#else
        return false;
#endif
    }

    // 기본 로그 메서드
    public static void Log(string message, DebugColor color = DebugColor.Default, int fontSize = 12)
    {
        if (!isLoggingEnabled) return;
        Color unityColor = GetColor(color);
        Log(message, unityColor, fontSize);
    }

    // 메서드 이름과 함께 부분적으로 스타일링된 로그 메서드
    public static void LogPartialStyled(string methodName, string message, Color color, int fontSize = 12, int baseFontSize = 13)
    {
        if (!isLoggingEnabled) return;
        string colorHtml = ColorUtility.ToHtmlStringRGB(color);
        string styledMethodName = $"<size={fontSize}><color=#{colorHtml}>[{methodName}]</color></size>";
        string fullMessage = $"{styledMethodName} {message}";
        Debug.Log(fullMessage);
    }

    // 경고 로그 메서드
    public static void LogWarning(string message, DebugColor color = DebugColor.Default, int fontSize = 12)
    {
        if (!isLoggingEnabled) return;
        Color unityColor = GetColor(color);
        LogWarning(message, unityColor, fontSize);
    }

    // 에러 로그 메서드
    public static void LogError(string message, DebugColor color = DebugColor.Default, int fontSize = 12)
    {
        if (!isLoggingEnabled) return;
        Color unityColor = GetColor(color);
        LogError(message, unityColor, fontSize);
    }

    // 타이머 시작 로그 메서드
    public static System.Diagnostics.Stopwatch StartTimer()
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        return stopwatch;
    }

    // 타이머 종료 후 시간 로그 메서드
    public static void LogTime(System.Diagnostics.Stopwatch stopwatch, string message, DebugColor color = DebugColor.Default, int fontSize = 13)
    {
        stopwatch.Stop();
        string formattedMessage = $"{message} - Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        Log(formattedMessage, color, fontSize);
    }

    // 조건부 로그 메서드
    public static void LogIf(bool condition, string message, DebugColor color = DebugColor.Default, int fontSize = 14)
    {
        if (condition)
        {
            Log(message, color, fontSize);
        }
    }

    // 오브젝트 상태 로그 메서드
    public static void LogObjectState<T>(T obj, string messagePrefix = "", DebugColor color = DebugColor.Cyan, int fontSize = 14)
    {
        string json = JsonUtility.ToJson(obj, prettyPrint: true);
        string message = string.IsNullOrEmpty(messagePrefix) ? json : $"{messagePrefix}: {json}";
        Log(message, color, fontSize);
    }

    // 구분선 로그 메서드
    public static void LogSeparator(DebugColor color = DebugColor.Default, int fontSize = 14)
    {
        Log(new string('-', 50), color, fontSize);
    }

    // 내부 경고 로그 메서드
    private static void Log(string message, Color color, int fontSize)
    {
        if (!isLoggingEnabled) return;

        // 색상이 기본값(White)이 아닌 경우, 전달된 색상을 그대로 사용
        if (color == Color.white)
        {
            // 에디터에서는 회색, 빌드에서는 검정색으로 설정
#if UNITY_EDITOR
            color = new Color(0.75f, 0.75f, 0.75f); // 회색 (Gray)
#else
        color = Color.black; // 검정색 (Black)
#endif
        }

        // 색상을 HTML 형식으로 변환
        string colorHtml = ColorUtility.ToHtmlStringRGB(color);

        // 텍스트에 크기와 색상 적용
        string formattedMessage = $"<size={fontSize}><color=#{colorHtml}>{message}</color></size>";

        // Unity의 Debug.Log 메서드를 사용하여 메시지를 출력
        Debug.Log(formattedMessage);
    }

    // 내부 경고 로그 메서드
    private static void LogWarning(string message, Color color, int fontSize)
    {
        string colorHtml = ColorUtility.ToHtmlStringRGB(color);
        string formattedMessage = $"<size={fontSize}><color=#{colorHtml}>{message}</color></size>";
        Debug.LogWarning(formattedMessage);
    }

    // 내부 에러 로그 메서드
    private static void LogError(string message, Color color, int fontSize)
    {
        // 에디터에서는 회색, 빌드에서는 검정색으로 설정
#if UNITY_EDITOR
        color = new Color(0.75f, 0.75f, 0.75f); // 회색 (Gray)
#else
    color = Color.black; // 검정색 (Black)
#endif
        string colorHtml = ColorUtility.ToHtmlStringRGB(color);
        string formattedMessage = $"<size={fontSize}><color=#{colorHtml}>{message}</color></size>";
        Debug.LogError(formattedMessage);
    }
}