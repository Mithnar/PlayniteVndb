﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace VndbSharp.Extensions
{
	internal static class TaskExtensions
	{

		public static async Task TimeoutAfter(this Task task, TimeSpan timeout,
			CancellationTokenSource cancellationTokenSource = null)
		{
			if (timeout == TimeSpan.Zero)
				throw new TimeoutException();

			if (cancellationTokenSource == null)
				cancellationTokenSource = new CancellationTokenSource();

			var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cancellationTokenSource.Token))
				.ConfigureAwait(false);
			cancellationTokenSource.Cancel();

			if (completedTask != task)
				throw new TimeoutException();
		}

		public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout,
			CancellationTokenSource cancellationTokenSource = null)
		{
			if (timeout == TimeSpan.Zero)
				throw new TimeoutException();

			if (cancellationTokenSource == null)
				cancellationTokenSource = new CancellationTokenSource();
			
			var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cancellationTokenSource.Token))
				.ConfigureAwait(false);
			cancellationTokenSource.Cancel();

			if (completedTask != task)
				throw new TimeoutException();

			return await task;
		}

	}
}
