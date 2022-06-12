using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keryhe.Pipelines.Steps
{
    public abstract class Step<T> : IStep<T>
    {
        private readonly BlockingCollection<T> _buffer;

        public event EventHandler<StepEventArgs<T>>? StepStarted;
        public event EventHandler<StepEventArgs<T>>? StepCompleted;
        public event EventHandler<StepEventArgs<T>>? StepCanceled;
        public event EventHandler<StepExceptionEventArgs<T>>? StepFailed;

        public Step()
        {
            _buffer = new BlockingCollection<T>();
        }

        public void Start(Action<T, int> callback, int stepIndex, CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while(!cancellationToken.IsCancellationRequested)
                {
                    int nextIndex = stepIndex + 1;
                    try
                    {
                        var data = _buffer.Take(cancellationToken);

                        try
                        {
                            OnStepStarted(new StepEventArgs<T>(this.GetType().Name, data));
                            bool status = Run(ref data);
                            if (status)
                            {
                                OnStepCompleted(new StepEventArgs<T>(this.GetType().Name, data));
                                callback(data, nextIndex);
                            }
                            else
                            {
                                OnStepCanceled(new StepEventArgs<T>(this.GetType().Name, data));
                                callback(data, -1);
                            }
                        }
                        catch (Exception ex)
                        {
                            OnStepFailed(new StepExceptionEventArgs<T>(this.GetType().Name, data, ex));
                        }
                    }
                    catch(OperationCanceledException)
                    {
                        // Ignore Error
                    }
                    
                }
            });
        }

        public void Execute(T data)
        {
            _buffer.Add(data);
        }

        public abstract bool Run(ref T data);

        private void OnStepStarted(StepEventArgs<T> e)
        {
            StepStarted?.Invoke(this, e);
        }

        private void OnStepCompleted(StepEventArgs<T> e)
        {
            StepCompleted?.Invoke(this, e);
        }

        private void OnStepCanceled(StepEventArgs<T> e)
        {
            StepCanceled?.Invoke(this, e);
        }

        private void OnStepFailed(StepExceptionEventArgs<T> e)
        {
            StepFailed?.Invoke(this, e);
        }
    }
}
