using Keryhe.Pipelines.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keryhe.Pipelines
{
    public interface IPipeline<T>
    {
        void AddStep(IStep<T> step);
        void Start(CancellationToken cancellationToken);
        void Execute(T data);

        event EventHandler<PipelineEventArgs<T>> PipelineStarted;
        event EventHandler<PipelineEventArgs<T>> PipelineCompleted;
    }

    public class PipelineEventArgs<T> : EventArgs
    {
        public PipelineEventArgs(string pipelineName, T data)
        {
            PipelineName = pipelineName;
            Data = data;
        }

        public string PipelineName { get; set; }
        public T Data { get; set; }
    }
}
