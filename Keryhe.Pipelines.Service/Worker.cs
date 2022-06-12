namespace Keryhe.Pipelines.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Pipeline<string> pipeline = new Pipeline<string>("TestPipeline");
            pipeline.PipelineStarted += Pipeline_Started;
            pipeline.PipelineCompleted += Pipeline_Completed;

            Steps.StringTest1 step1 = new Steps.StringTest1();
            Steps.StringTest2 step2 = new Steps.StringTest2();

            step1.StepStarted += Step_Started;
            step1.StepCompleted += Step_Completed;
            step1.StepCanceled += Step_Canceled;
            step1.StepFailed += Step_Failed;

            step2.StepStarted += Step_Started;
            step2.StepCompleted += Step_Completed;

            pipeline.AddStep(step1);
            pipeline.AddStep(step2);

            pipeline.Start(stoppingToken);

            pipeline.Execute("InitialData1");
            pipeline.Execute("CancelData");
            pipeline.Execute("InitialData2");
            pipeline.Execute("FailData");
            pipeline.Execute("InitialData3");

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            step1.StepStarted -= Step_Started;
            step2.StepCompleted -= Step_Completed;
            pipeline.PipelineStarted -= Pipeline_Started;
            pipeline.PipelineCompleted -=Pipeline_Completed;
        }

        private void Step_Failed(object? sender, Pipelines.Steps.StepExceptionEventArgs<string> e)
        {
            Console.WriteLine(e.StepName + " Failed : " + e.Data);
        }

        private void Step_Canceled(object? sender, Pipelines.Steps.StepEventArgs<string> e)
        {
            Console.WriteLine(e.StepName + " Canceled : " + e.Data);
        }

        private void Step_Completed(object? sender, Pipelines.Steps.StepEventArgs<string> e)
        {
            Console.WriteLine(e.StepName + " Completed : " + e.Data);
        }

        private void Step_Started(object? sender, Pipelines.Steps.StepEventArgs<string> e)
        {
            Console.WriteLine(e.StepName + " Started : " + e.Data);
        }

        private void Pipeline_Completed(object? sender, PipelineEventArgs<string> e)
        {
            Console.WriteLine(e.PipelineName + " Completed : " + e.Data);
        }

        private void Pipeline_Started(object? sender, PipelineEventArgs<string> e)
        {
            Console.WriteLine(e.PipelineName + " Started : " + e.Data);
        }
    }
}