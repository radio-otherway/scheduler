namespace OtherWay.Radio.Scheduler.Services.Extensions;

public static class DateTimeExtensions {
  public static string ToLongDateTimeString(this DateTime date) {
    return $"{date.ToLongDateString()} {date.ToLongTimeString()}";
  }
}