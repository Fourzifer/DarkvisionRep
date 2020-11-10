

using System.Collections.Generic;

public static class Utility {

	public interface IObserver<NotificationType> {
		void Notify(NotificationType type);
	}

	public static void NotifyObservers<NotificationType>(List<IObserver<NotificationType>> observers, NotificationType notification) {
		foreach (var observer in observers) {
			observer.Notify(notification);
		}
	}

}
