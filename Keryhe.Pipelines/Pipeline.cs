using Keryhe.Pipelines.Steps;

namespace Keryhe.Pipelines
{
    public class Pipeline<T> : IPipeline<T>
    {
        private readonly List<IStep<T>> _steps;
        private readonly string _name;

        public event EventHandler<PipelineEventArgs<T>>? PipelineStarted;
        public event EventHandler<PipelineEventArgs<T>>? PipelineCompleted;
        public event EventHandler<PipelineEventArgs<T>>? PipelineCanceled;

        public Pipeline(string name)
        { 
            _steps = new List<IStep<T>>();
            _name = name;
        }

        public void AddStep(IStep<T> step)
        {
            _steps.Add(step);
        }

        public void Start(CancellationToken cancellationToken)
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                _steps[i].Start(ExecuteNextStep, i, cancellationToken);
            }
        }

        public void Execute(T data)
        {
            if (_steps.Any())
            {
                OnPipelineStarted(new PipelineEventArgs<T>(_name, data));
                _steps[0].Execute(data);
            }
        }    

        private void ExecuteNextStep(T data, int stepIndex)
        {
            if(stepIndex < 0)
            {
                OnPipelineCanceled(new PipelineEventArgs<T>(_name, data));
            }
            else if (stepIndex < _steps.Count)
            {
                _steps[stepIndex].Execute(data);
            }
            else
            {
                OnPipelineCompleted(new PipelineEventArgs<T>(_name, data));
            }
        }

        private void OnPipelineStarted(PipelineEventArgs<T> e)
        {
            PipelineStarted?.Invoke(this, e);
        }

        private void OnPipelineCompleted(PipelineEventArgs<T> e)
        {
            PipelineCompleted?.Invoke(this, e);
        }

        private void OnPipelineCanceled(PipelineEventArgs<T> e)
        {
            PipelineCanceled?.Invoke(this, e);
        }
    }
}