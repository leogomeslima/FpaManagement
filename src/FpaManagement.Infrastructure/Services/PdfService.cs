using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Domain.Entities;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FpaManagement.Infrastructure.Services;

public class PdfService : IPdfService
{
    private readonly ILogger<PdfService> _logger;

    public PdfService(ILogger<PdfService> logger)
    {
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateReportAsync<T>(T data, string templateName, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeItem().Text($"Relatório - {templateName}")
                                .SemiBold().FontSize(16);

                            row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                                .FontSize(10);
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            column.Item().Text("Conteúdo do relatório")
                                .FontSize(14);

                            // Aqui viria a lógica específica para cada tipo de relatório
                            column.Item().Text(data?.ToString() ?? "Sem dados");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }, cancellationToken);
    }

    public byte[] GenerateBudgetReport(Budget budget, BudgetVersion version)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);

                page.Header()
                    .Column(column =>
                    {
                        column.Spacing(5);

                        column.Item().Text("RELATÓRIO DE ORÇAMENTO")
                            .Bold().FontSize(18).FontColor(Colors.Blue.Medium);

                        column.Item().Text($"{budget.Name} - {budget.FiscalYear}")
                            .FontSize(14);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Versão: {version.VersionName} (v{version.VersionNumber})");
                            row.RelativeItem().AlignRight().Text($"Status: {budget.Status}");
                        });

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    });

                page.Content()
                    .PaddingVertical(10)
                    .Column(column =>
                    {
                        // Totais
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Background(Colors.Grey.Lighten3)
                                .Padding(10).Column(totalCol =>
                                {
                                    totalCol.Item().Text("TOTAL DO ORÇAMENTO").Bold();
                                    totalCol.Item().Text($"{version.CalculateTotal():C}")
                                        .FontSize(20).Bold().FontColor(Colors.Green.Darken2);
                                });
                        });

                        column.Spacing(10);

                        // Tabela de itens
                        column.Item().Text("ITENS DO ORÇAMENTO").Bold().FontSize(14);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Categoria
                                columns.RelativeColumn(4); // Descrição
                                columns.RelativeColumn(2); // Centro de Custo
                                columns.RelativeColumn(2); // Valor
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Text("Categoria").Bold();
                                header.Cell().Text("Descrição").Bold();
                                header.Cell().Text("Centro de Custo").Bold();
                                header.Cell().AlignRight().Text("Valor").Bold();

                                header.Cell().ColumnSpan(4).BorderBottom(1).BorderColor(Colors.Black);
                            });

                            // Data
                            foreach (var item in version.Items.OrderBy(i => i.Category))
                            {
                                table.Cell().Text(item.Category);
                                table.Cell().Text(item.Description);
                                table.Cell().Text(item.CostCenter?.Code ?? "-");
                                table.Cell().AlignRight().Text($"{item.PlannedAmount:C}");
                            }

                            // Footer with totals
                            table.Footer(footer =>
                            {
                                footer.Cell().ColumnSpan(3).AlignRight().Text("TOTAL:").Bold();
                                footer.Cell().AlignRight().Text($"{version.CalculateTotal():C}").Bold();
                            });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Relatório gerado em ");
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                        text.Span(" | Página ");
                        text.CurrentPageNumber();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateCashFlowReport(IEnumerable<CashFlowEntry> entries, DateTime startDate, DateTime endDate)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text("RELATÓRIO DE FLUXO DE CAIXA")
                            .Bold().FontSize(18).FontColor(Colors.Blue.Medium);

                        column.Item().Text($"Período: {startDate:dd/MM/yyyy} a {endDate:dd/MM/yyyy}")
                            .FontSize(12);

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    });

                page.Content()
                    .PaddingVertical(10)
                    .Column(column =>
                    {
                        var entriesList = entries.ToList();
                        var totalInflow = entriesList.Where(e => e.Type == Domain.Enums.CashFlowType.Inflow).Sum(e => e.Amount.Amount);
                        var totalOutflow = entriesList.Where(e => e.Type == Domain.Enums.CashFlowType.Outflow).Sum(e => e.Amount.Amount);
                        var netCashFlow = totalInflow - totalOutflow;

                        // Resumo
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Background(Colors.Green.Lighten4)
                                .Padding(10).Column(col =>
                                {
                                    col.Item().Text("ENTRADAS").Bold();
                                    col.Item().Text($"{totalInflow:C}").FontSize(16).FontColor(Colors.Green.Darken2);
                                });

                            row.RelativeItem().Background(Colors.Red.Lighten4)
                                .Padding(10).Column(col =>
                                {
                                    col.Item().Text("SAÍDAS").Bold();
                                    col.Item().Text($"{totalOutflow:C}").FontSize(16).FontColor(Colors.Red.Darken2);
                                });

                            row.RelativeItem().Background(Colors.Blue.Lighten4)
                                .Padding(10).Column(col =>
                                {
                                    col.Item().Text("SALDO").Bold();
                                    col.Item().Text($"{netCashFlow:C}")
                                        .FontSize(16)
                                        .FontColor(netCashFlow >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2);
                                });
                        });

                        column.Spacing(15);

                        // Tabela de lançamentos
                        column.Item().Text("LANÇAMENTOS").Bold().FontSize(14);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Data
                                columns.RelativeColumn(3); // Descrição
                                columns.RelativeColumn(2); // Categoria
                                columns.RelativeColumn(2); // Tipo
                                columns.RelativeColumn(2); // Valor
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Data").Bold();
                                header.Cell().Text("Descrição").Bold();
                                header.Cell().Text("Categoria").Bold();
                                header.Cell().Text("Tipo").Bold();
                                header.Cell().AlignRight().Text("Valor").Bold();
                            });

                            foreach (var entry in entriesList.OrderBy(e => e.Date))
                            {
                                table.Cell().Text(entry.Date.ToString("dd/MM/yyyy"));
                                table.Cell().Text(entry.Description);
                                table.Cell().Text(entry.Category?.Name ?? "-");
                                table.Cell().Text(entry.Type.ToString());
                                table.Cell().AlignRight().Text($"{entry.Amount:C}")
                                    .FontColor(entry.Type == Domain.Enums.CashFlowType.Inflow ? Colors.Green.Darken2 : Colors.Red.Darken2);
                            }
                        });
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateDfcReport(IEnumerable<DfcEntry> entries, int year)
    {
        // Implementação similar aos outros relatórios
        throw new NotImplementedException();
    }
}
