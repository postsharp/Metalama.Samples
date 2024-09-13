using System.ComponentModel.DataAnnotations;

public interface IReport
{
    [Required]
    string Title { get; }

    IReadOnlyList<IReportAuthor> Authors { get; }

    IReadOnlyList<IReportSection> Section { get; }
}

public interface IReportAuthor
{
    [Required]
    string Name { get; }

    [Required]
    string Email { get; }
}

public interface IReportSection
{
    string Header { get; }

    string Footnote { get; }
}

public interface IReportTextSection : IReportSection
{
    string Text { get; }
}

public interface IReportListSection : IReportSection 
{ 
    IReadOnlyList<string> Items { get; }
}

public interface IBuilder<T>
{
    T Build();
}

public interface IBuilderFactory
{
    TBuilder CreateBuilder<TBuilt, TBuilder>()
        where TBuilder : IBuilder<TBuilt>;
}

public interface IReportBuilder : IBuilder<IReport>
{
    string Title { get; set; }

    IList<IReportAuthorBuilder> Authors { get; }

    IList<IReportSectionBuilder> Sections { get; }
}

public interface IReportAuthorBuilder : IBuilder<IReportAuthor>
{
    string Name { get; set; }

    string Email { get; set; }
}

public interface IReportSectionBuilder : IBuilder<IReportSection>
{
    string Header { get; set; }

    string FootNote { get; set; }
}

public interface IReportTextSectionBuilder : IReportSectionBuilder, IBuilder<IReportTextSection>
{

}

public sealed class ReportBuildingDirector
{
    private readonly IBuilderFactory _source;

    public ReportBuildingDirector()
    {
    }

    public IReport CreatePerformanceReport(string authorName, string authorEmail, (string Name, double Value)[] performanceMetrics)
    {
        var reportBuilder = this._source.CreateBuilder<IReport, IReportBuilder>();
        reportBuilder.Title = $"{performanceCategory} Performance Report";

        var authorBuilder = this._source.CreateBuilder<IReportAuthor, IReportAuthorBuilder>();
        authorBuilder.Name = authorName;
        authorBuilder.Email = authorEmail;
        reportBuilder.Authors.Add(authorBuilder);

        var sectionBuilder = this._source.CreateBuilder<IReportSection, IReportSectionBuilder>();
        sectionBuilder.Header = "Summary";
        sectionBuilder.Content = $"This is a performance report for {string.Join(performanceMetrics.)}.";
        reportBuilder.Sections.Add(sectionBuilder);

        return reportBuilder.Build();
    }
}