

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

	public static void Register<NotificationType>(this IObserver<NotificationType> observer, List<IObserver<NotificationType>> observerList) {
		observerList.Add(observer);
	}

	public static void Deregister<NotificationType>(this IObserver<NotificationType> observer, List<IObserver<NotificationType>> observerList) {
		observerList.Remove(observer);
	}


}
