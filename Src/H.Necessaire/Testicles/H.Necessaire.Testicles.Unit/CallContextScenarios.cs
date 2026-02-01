using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class CallContextScenarios
    {
        const string ambientDataKey = "AmbientData";
        public CallContextScenarios()
        {

        }

        [Fact(DisplayName = "CallContext Works In SingleThread No Async")]
        public void CallContext_Works_In_SingleThread_No_Async()
        {
            CallContext<string>.SetData(ambientDataKey, "SomeValue");

            AssertExpectedContextValueInCallChain("SomeValue", "In a non-async-await, non-multi-threaded call, ambient data is the last one set");
        }

        [Fact(DisplayName = "CallContext Works For Multi Threaded Context")]
        public void CallContext_Works_For_Multi_Threaded_Context()
        {
            int numberOfThreads = 20;

            SemaphoreSlim semaphore = new SemaphoreSlim(0, numberOfThreads);
            SemaphoreSlim mainSemaphore = new SemaphoreSlim(0, 1);


            foreach (int index in Enumerable.Range(0, numberOfThreads))
            {
                new Thread(() =>
                {
                    CallContext<string>.SetData(ambientDataKey, $"SomeValue{index}");

                    AssertExpectedContextValueInCallChain($"SomeValue{index}", $"Thread # {index} should have context value [SomeValue{index}]");

                    int prevCount = semaphore.Release();

                    if (prevCount == 1)
                        mainSemaphore.Release();

                }).Start();
            }

            mainSemaphore.Wait(TestContext.Current.CancellationToken);
        }

        [Fact(DisplayName = "CallContext Works For Async Await On Main Thread")]
        public async Task CallContext_Works_For_Async_Await_On_Main_Thread()
        {
            CallContext<string>.SetData(ambientDataKey, "SomeValue");

            await AssertExpectedContextValueInCallChainAsync("SomeValue", "In an async-await, non-multi-threaded call, ambient data is the last one set");


            await Task.WhenAll(
                new Func<Task>(async () => {
                    CallContext<string>.SetData(ambientDataKey, "SomeValueA");
                    await AssertExpectedContextValueInCallChainAsync("SomeValueA", "That's the value for this async-await branch");
                })(),
                new Func<Task>(async () => {
                    CallContext<string>.SetData(ambientDataKey, "SomeValueB");
                    await AssertExpectedContextValueInCallChainAsync("SomeValueB", "That's the value for this async-await branch");
                })(),
                new Func<Task>(async () => {
                    CallContext<string>.SetData(ambientDataKey, "SomeValueC");
                    await AssertExpectedContextValueInCallChainAsync("SomeValueC", "That's the value for this async-await branch");
                })(),
                new Func<Task>(async () => {

                    await AssertExpectedContextValueInCallChainAsync("SomeValue", "On this async-await branch we did not override the value");

                })()
            );

            await Task.WhenAll(
                Task.Run(async () => {
                    CallContext<string>.SetData(ambientDataKey, "SomeValueA");
                    await AssertExpectedContextValueInCallChainAsync("SomeValueA", "That's the value for this async-await branch");
                }, TestContext.Current.CancellationToken),
                Task.Run(async () => {
                    CallContext<string>.SetData(ambientDataKey, "SomeValueB");
                    await AssertExpectedContextValueInCallChainAsync("SomeValueB", "That's the value for this async-await branch");
                }, TestContext.Current.CancellationToken),
                Task.Run(async () => {
                    CallContext<string>.SetData(ambientDataKey, "SomeValueC");
                    await AssertExpectedContextValueInCallChainAsync("SomeValueC", "That's the value for this async-await branch");
                }, TestContext.Current.CancellationToken),
                Task.Run(async () => {

                    await AssertExpectedContextValueInCallChainAsync("SomeValue", "On this async-await branch we did not override the value");

                }, TestContext.Current.CancellationToken)
            );
        }

        [Fact(DisplayName = "CallContext Works For Async Await On Multiple Threads")]
        public async Task CallContext_Works_For_Async_Await_On_Multiple_Threads()
        {
            int numberOfThreads = 20;

            SemaphoreSlim semaphore = new SemaphoreSlim(0, numberOfThreads);
            SemaphoreSlim mainSemaphore = new SemaphoreSlim(0, 1);


            foreach (int index in Enumerable.Range(0, numberOfThreads))
            {
                new Thread(async () =>
                {
                    CallContext<string>.SetData(ambientDataKey, $"SomeValue{index}");

                    await AssertExpectedContextValueInCallChainAsync($"SomeValue{index}", $"Thread # {index} should have context value [SomeValue{index}]");

                    int prevCount = semaphore.Release();

                    if (prevCount == 1)
                        mainSemaphore.Release();

                }).Start();
            }

            await mainSemaphore.WaitAsync(TestContext.Current.CancellationToken);

        }

        static string GetCurrentCallContextAmbientData() => CallContext<string>.GetData(ambientDataKey);

        static void AssertExpectedContextValueInCallChain(string expectedValue, string reason = null)
        {
            new Action(() =>
            {
                new Action(() =>
                {
                    new Action(() =>
                    {
                        string contextValue = GetCurrentCallContextAmbientData();
                        contextValue.Should().Be(expectedValue, because: reason);

                    })();

                })();

            })();
        }

        static async Task AssertExpectedContextValueInCallChainAsync(string expectedValue, string reason = null)
        {
            await new Func<Task>(async () =>
            {

                await new Func<Task>(async () =>
                {

                    await new Func<Task>(async () =>
                    {

                        await new Func<Task>(() =>
                        {

                            string contextValue = GetCurrentCallContextAmbientData();
                            contextValue.Should().Be(expectedValue, because: reason);

                            return Task.CompletedTask;

                        })();

                    })();

                })();

            })();
        }
    }
}
