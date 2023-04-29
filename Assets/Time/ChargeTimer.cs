using System.Threading.Tasks;

// Simple helper class to wait some amount of ticks and report the percentage of
// ticks elapsed.
public class ChargeTimer {
  int StartTick;
  public float ElapsedPercent { get; private set; }
  public async Task Ticks(TaskScope scope, int ticks) {
    StartTick = Timeval.TickCount;
    try {
      await scope.Ticks(ticks);
    } finally {
      ElapsedPercent = (Timeval.TickCount - StartTick) / (float)ticks;
    }
  }
}
