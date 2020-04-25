namespace Carter.App.Validation.FrameData
{
    using FluentValidation;

    public class FrameData
    {
        #pragma warning disable SA1516, SA1300
        public FrameInfo frameInfo { get; set; }
        #pragma warning restore SA151, SA1300
    }

    public class FrameInfo
    {
        #pragma warning disable SA1516, SA1300
        public float realtimeSinceStartup { get; set; }
        #pragma warning restore SA151, SA1300
    }

    public class FrameDataValidator : AbstractValidator<FrameData>
    {
        public FrameDataValidator()
        {
            this.RuleFor(x => x.frameInfo.realtimeSinceStartup)
                .NotNull()
                .When(x => x.frameInfo != null);
        }
    }
}