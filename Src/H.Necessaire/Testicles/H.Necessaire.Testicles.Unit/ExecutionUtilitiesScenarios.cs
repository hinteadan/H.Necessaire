using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class ExecutionUtilitiesScenarios
    {
        private const double timingToleranceInMilliseconds = 80;

        [Fact(DisplayName = "TryAFewTimesOrFailWithGrace Doesnt Retry If Call Succeeds")]
        public void TryAFewTimesOrFailWithGrace_Doesnt_Retry_If_Call_Succeeds()
        {
            int callCount = 0;
            int retryCallCount = 0;
            int failCallCount = 0;

            new Action(() =>
            {
                callCount++;
            })
            .TryOrFailWithGrace(
                numberOfTimes: 5,
                onFail: x => failCallCount++,
                onRetry: x => retryCallCount++,
                millisecondsToSleepBetweenRetries: 0
            );

            callCount.Should().Be(1);
            failCallCount.Should().Be(0);
            retryCallCount.Should().Be(0);
        }

        [Fact(DisplayName = "TryAFewTimesOrFailWithGrace Async Awaits and Doesnt Retry If Call Succeeds")]
        public async Task TryAFewTimesOrFailWithGraceAsync_Awaits_And_Doesnt_Retry_If_Call_Succeeds()
        {
            int callCount = 0;
            int retryCallCount = 0;
            int failCallCount = 0;

            DateTime startedOn = DateTime.Now;

            await new Func<Task>(async () =>
            {
                callCount++;
                await Task.Delay(200);
            })
            .TryOrFailWithGrace(
                numberOfTimes: 5,
                onFail: x => failCallCount++,
                onRetry: x => retryCallCount++,
                millisecondsToSleepBetweenRetries: 0
            );

            DateTime endedOn = DateTime.Now;

            TimeSpan duration = endedOn - startedOn;

            duration.TotalMilliseconds.Should().BeApproximately(200, timingToleranceInMilliseconds, "The call is awaited");
            callCount.Should().Be(1);
            failCallCount.Should().Be(0);
            retryCallCount.Should().Be(0);
        }

        [Fact(DisplayName = "TryAFewTimesOrFailWithGrace Calls Retry Action On Retry")]
        public void TryAFewTimesOrFailWithGrace_Calls_Retry_Action_On_Retry()
        {
            int callCount = 0;
            int retryCallCount = 0;
            int failCallCount = 0;

            new Action(() =>
            {
                callCount++;

                if (callCount <= 2)
                    throw new InvalidOperationException("Eat a dick, gravity!");
            })
            .TryOrFailWithGrace(
                numberOfTimes: 5,
                onFail: x => failCallCount++,
                onRetry: x => retryCallCount++,
                millisecondsToSleepBetweenRetries: 0
            );

            callCount.Should().Be(3);
            failCallCount.Should().Be(0);
            retryCallCount.Should().Be(2);
        }

        [Fact(DisplayName = "TryAFewTimesOrFailWithGrace Async Awaits and Calls Retry Action On Retry")]
        public async Task TryAFewTimesOrFailWithGraceAsync_Awaits_And_Calls_Retry_Action_On_Retry()
        {
            int callCount = 0;
            int retryCallCount = 0;
            int failCallCount = 0;

            DateTime startedOn = DateTime.Now;

            await new Func<Task>(async () =>
            {
                await Task.Delay(200);

                callCount++;

                if (callCount <= 2)
                    throw new InvalidOperationException("Eat a dick, gravity!");
            })
            .TryOrFailWithGrace(
                numberOfTimes: 5,
                onFail: x => failCallCount++,
                onRetry: x => retryCallCount++,
                millisecondsToSleepBetweenRetries: 0
            );

            DateTime endedOn = DateTime.Now;

            TimeSpan duration = endedOn - startedOn;

            duration.TotalMilliseconds.Should().BeApproximately(200 * 3, timingToleranceInMilliseconds, "The call is awaited");
            callCount.Should().Be(3);
            failCallCount.Should().Be(0);
            retryCallCount.Should().Be(2);
        }

        [Fact(DisplayName = "TryAFewTimesOrFailWithGrace Calls Fail Action On Fail")]
        public void TryAFewTimesOrFailWithGrace_Calls_Fail_Action_On_Fail()
        {
            int callCount = 0;
            int retryCallCount = 0;
            int failCallCount = 0;

            new Action(() =>
            {
                callCount++;
                throw new InvalidOperationException("Eat a dick, gravity!");
            })
            .TryOrFailWithGrace(
                numberOfTimes: 5,
                onFail: x => failCallCount++,
                onRetry: x => retryCallCount++,
                millisecondsToSleepBetweenRetries: 0
            );

            callCount.Should().Be(5);
            failCallCount.Should().Be(1);
            retryCallCount.Should().Be(4);
        }

        [Fact(DisplayName = "TryAFewTimesOrFailWithGrace Async Awaits and Calls Fail Action On Fail")]
        public async Task TryAFewTimesOrFailWithGraceAsync_Awaits_And_Calls_Fail_Action_On_Fail()
        {
            int callCount = 0;
            int retryCallCount = 0;
            int failCallCount = 0;

            DateTime startedOn = DateTime.Now;

            await new Func<Task>(async () =>
            {
                await Task.Delay(100);
                callCount++;
                throw new InvalidOperationException("Eat a dick, gravity!");
            })
            .TryOrFailWithGrace(
                numberOfTimes: 5,
                onFail: x => failCallCount++,
                onRetry: x => retryCallCount++,
                millisecondsToSleepBetweenRetries: 0
            );

            DateTime endedOn = DateTime.Now;

            TimeSpan duration = endedOn - startedOn;

            duration.TotalMilliseconds.Should().BeApproximately(100 * 5, timingToleranceInMilliseconds * 1.75, "The call is awaited");
            callCount.Should().Be(5);
            failCallCount.Should().Be(1);
            retryCallCount.Should().Be(4);
        }

        [Fact(DisplayName = "TryAFewTimesOrFailWithGrace Fail And Retry Actions Dont Break Execution No Matter What")]
        public void TryAFewTimesOrFailWithGrace_Fail_And_Retry_Actions_Dont_Break_Execution_No_Matter_What()
        {
            int callCount = 0;
            int retryCallCount = 0;
            int failCallCount = 0;

            new Action(() =>
            {
                callCount++;
                throw new InvalidOperationException("Eat a dick, gravity!");
            })
            .TryOrFailWithGrace(
                numberOfTimes: 5,
                onFail: x => { failCallCount++; throw new InvalidOperationException("Eat more dick, gravity!"); },
                onRetry: x => { retryCallCount++; throw new InvalidOperationException("Eat more dick, gravity!"); },
                millisecondsToSleepBetweenRetries: 0
            );

            callCount.Should().Be(5);
            failCallCount.Should().Be(1);
            retryCallCount.Should().Be(4);
        }

        [Fact(DisplayName = "TryAFewTimesOrFailWithGrace Async Awaits And Fail And Retry Actions Dont Break Execution No Matter What")]
        public async Task TryAFewTimesOrFailWithGraceAsync_Awaits_And_Fail_And_Retry_Actions_Dont_Break_Execution_No_Matter_What()
        {
            int callCount = 0;
            int retryCallCount = 0;
            int failCallCount = 0;

            DateTime startedOn = DateTime.Now;

            await new Func<Task>(async () =>
            {
                await Task.Delay(100);
                callCount++;
                throw new InvalidOperationException("Eat a dick, gravity!");
            })
            .TryOrFailWithGrace(
                numberOfTimes: 5,
                onFail: x => { failCallCount++; throw new InvalidOperationException("Eat more dick, gravity!"); },
                onRetry: x => { retryCallCount++; throw new InvalidOperationException("Eat more dick, gravity!"); },
                millisecondsToSleepBetweenRetries: 0
            );

            DateTime endedOn = DateTime.Now;

            TimeSpan duration = endedOn - startedOn;

            duration.TotalMilliseconds.Should().BeApproximately(100 * 5, timingToleranceInMilliseconds * 2, "The call is awaited");
            callCount.Should().Be(5);
            failCallCount.Should().Be(1);
            retryCallCount.Should().Be(4);
        }
    }
}
