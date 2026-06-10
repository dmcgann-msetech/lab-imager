using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LabImager.Services.Help;

namespace LabImager.Views
{
    public partial class HelpWindow : Window
    {
        private readonly MarkdownHelpContentService _helpContentService;

        private static readonly SolidColorBrush PageBackground = Brush("#0B0F12");
        private static readonly SolidColorBrush PanelBackground = Brush("#101820");
        private static readonly SolidColorBrush AccentBlue = Brush("#38BDF8");
        private static readonly SolidColorBrush AccentMuted = Brush("#0EA5E9");
        private static readonly SolidColorBrush TextPrimary = Brush("#E5E7EB");
        private static readonly SolidColorBrush TextBody = Brush("#CBD5E1");
        private static readonly SolidColorBrush TextMuted = Brush("#94A3B8");
        private static readonly SolidColorBrush BorderMuted = Brush("#334155");
        private static readonly SolidColorBrush CodeBackground = Brush("#111827");

        public HelpWindow()
        {
            InitializeComponent();

            _helpContentService = new MarkdownHelpContentService();
            LoadLogo();
            HelpNavigation.SelectedIndex = 0;
        }

        private void HelpNavigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HelpNavigation.SelectedItem is not ListBoxItem item || item.Tag is not string tag)
            {
                return;
            }

            AboutPanel.Visibility = Visibility.Collapsed;
            DocumentPanel.Visibility = Visibility.Visible;

            switch (tag)
            {
                case "HELP":
                    LoadDocument("docs", "help", "HELP.md");
                    break;

                case "USERGUIDE":
                    LoadDocument("docs", "help", "USER-GUIDE.md");
                    break;

                case "FAQ":
                    LoadDocument("docs", "help", "FAQ.md");
                    break;

                case "SHORTCUTS":
                    LoadDocument("docs", "help", "KEYBOARD-SHORTCUTS.md");
                    break;

                case "RELEASE":
                    LoadDocument("docs", "releases", "RELEASE-NOTES.md");
                    break;

                case "ABOUT":
                    ShowAbout();
                    break;
            }
        }

        private void LoadDocument(params string[] pathParts)
        {
            var content = _helpContentService.LoadDocument(pathParts);
            DocumentViewer.Document = BuildProfessionalDocument(content);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (DocumentViewer.Document?.Blocks.FirstBlock is not null)
                {
                    DocumentViewer.Document.Blocks.FirstBlock.BringIntoView();
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private FlowDocument BuildProfessionalDocument(string content)
        {
            var document = new FlowDocument
            {
                Background = PageBackground,
                Foreground = TextPrimary,
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize = 14,
                PagePadding = new Thickness(0),
                ColumnWidth = 920,
                LineHeight = 22
            };

            var lines = content.Replace("\r\n", "\n").Split('\n');

            var inCodeBlock = false;
            var codeLines = new List<string>();

            for (var i = 0; i < lines.Length; i++)
            {
                var raw = lines[i];
                var line = raw.Trim();

                if (line.StartsWith("```"))
                {
                    if (!inCodeBlock)
                    {
                        inCodeBlock = true;
                        codeLines.Clear();
                    }
                    else
                    {
                        document.Blocks.Add(CreateCodeBlock(codeLines));
                        inCodeBlock = false;
                    }

                    continue;
                }

                if (inCodeBlock)
                {
                    codeLines.Add(raw);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.StartsWith("|") && line.EndsWith("|"))
                {
                    var tableLines = new List<string>();

                    while (i < lines.Length)
                    {
                        var tableLine = lines[i].Trim();

                        if (!(tableLine.StartsWith("|") && tableLine.EndsWith("|")))
                        {
                            i--;
                            break;
                        }

                        tableLines.Add(tableLine);
                        i++;
                    }

                    document.Blocks.Add(CreateMarkdownTable(tableLines));
                    continue;
                }

                if (line.StartsWith("# "))
                {
                    document.Blocks.Add(CreateH1(line[2..]));
                    continue;
                }

                if (line.StartsWith("## "))
                {
                    document.Blocks.Add(CreateH2(line[3..]));
                    continue;
                }

                if (line.StartsWith("### "))
                {
                    document.Blocks.Add(CreateH3(line[4..]));
                    continue;
                }

                if (line.StartsWith("* ") || line.StartsWith("- "))
                {
                    var listItems = new List<string> { line[2..] };

                    while (i + 1 < lines.Length)
                    {
                        var next = lines[i + 1].Trim();

                        if (next.StartsWith("* ") || next.StartsWith("- "))
                        {
                            listItems.Add(next[2..]);
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    document.Blocks.Add(CreateBulletList(listItems));
                    continue;
                }

                if (IsNumberedLine(line))
                {
                    var listItems = new List<string> { StripNumberPrefix(line) };

                    while (i + 1 < lines.Length)
                    {
                        var next = lines[i + 1].Trim();

                        if (IsNumberedLine(next))
                        {
                            listItems.Add(StripNumberPrefix(next));
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    document.Blocks.Add(CreateNumberedList(listItems));
                    continue;
                }

                if (line.StartsWith("---"))
                {
                    document.Blocks.Add(CreateDivider());
                    continue;
                }

                document.Blocks.Add(CreateBodyParagraph(line));
            }

            return document;
        }

        private Block CreateH1(string text)
        {
            var section = new Section
            {
                Margin = new Thickness(0, 0, 0, 24)
            };

            section.Blocks.Add(new Paragraph(new Run(CleanInlineMarkdown(text).ToUpperInvariant()))
            {
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                Foreground = TextPrimary,
                Margin = new Thickness(0, 0, 0, 8)
            });

            section.Blocks.Add(new Paragraph(new Run(" "))
            {
                BorderBrush = AccentBlue,
                BorderThickness = new Thickness(0, 0, 0, 2),
                Margin = new Thickness(0, 0, 0, 0),
                LineHeight = 1
            });

            return section;
        }

        private Block CreateH2(string text)
        {
            var section = new Section
            {
                Margin = new Thickness(0, 26, 0, 12)
            };

            section.Blocks.Add(new Paragraph(new Run(CleanInlineMarkdown(text)))
            {
                FontSize = 21,
                FontWeight = FontWeights.SemiBold,
                Foreground = AccentBlue,
                Margin = new Thickness(0, 0, 0, 4)
            });

            section.Blocks.Add(new Paragraph(new Run(" "))
            {
                BorderBrush = BorderMuted,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Margin = new Thickness(0, 0, 0, 0),
                LineHeight = 1
            });

            return section;
        }

        private Block CreateH3(string text)
        {
            return new Paragraph(new Run(CleanInlineMarkdown(text)))
            {
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = TextPrimary,
                Margin = new Thickness(0, 18, 0, 8)
            };
        }

        private Block CreateBodyParagraph(string text)
        {
            return new Paragraph(new Run(CleanInlineMarkdown(text)))
            {
                FontSize = 14,
                Foreground = TextBody,
                LineHeight = 22,
                Margin = new Thickness(0, 0, 0, 12)
            };
        }

        private Block CreateBulletList(IEnumerable<string> items)
        {
            var list = new List
            {
                MarkerStyle = TextMarkerStyle.Disc,
                Margin = new Thickness(22, 0, 0, 14),
                Foreground = TextBody
            };

            foreach (var item in items)
            {
                list.ListItems.Add(new ListItem(new Paragraph(new Run(CleanInlineMarkdown(item)))
                {
                    FontSize = 14,
                    LineHeight = 22,
                    Margin = new Thickness(0, 0, 0, 4)
                }));
            }

            return list;
        }

        private Block CreateNumberedList(IEnumerable<string> items)
        {
            var list = new List
            {
                MarkerStyle = TextMarkerStyle.Decimal,
                Margin = new Thickness(22, 0, 0, 14),
                Foreground = TextBody
            };

            foreach (var item in items)
            {
                list.ListItems.Add(new ListItem(new Paragraph(new Run(CleanInlineMarkdown(item)))
                {
                    FontSize = 14,
                    LineHeight = 22,
                    Margin = new Thickness(0, 0, 0, 4)
                }));
            }

            return list;
        }

        private Block CreateMarkdownTable(IReadOnlyList<string> rows)
        {
            var table = new Table
            {
                CellSpacing = 0,
                Margin = new Thickness(0, 8, 0, 18)
            };

            var parsedRows = rows
                .Where(r => !r.Contains("---"))
                .Select(r => r.Trim('|').Split('|').Select(c => CleanInlineMarkdown(c.Trim())).ToArray())
                .Where(r => r.Length > 0)
                .ToList();

            if (parsedRows.Count == 0)
            {
                return CreateBodyParagraph(string.Empty);
            }

            var columnCount = parsedRows.Max(r => r.Length);

            for (var c = 0; c < columnCount; c++)
            {
                table.Columns.Add(new TableColumn());
            }

            var group = new TableRowGroup();
            table.RowGroups.Add(group);

            for (var r = 0; r < parsedRows.Count; r++)
            {
                var row = new TableRow();
                group.Rows.Add(row);

                for (var c = 0; c < columnCount; c++)
                {
                    var value = c < parsedRows[r].Length ? parsedRows[r][c] : string.Empty;

                    row.Cells.Add(new TableCell(new Paragraph(new Run(value))
                    {
                        FontSize = 13,
                        Margin = new Thickness(0),
                        Foreground = r == 0 ? TextPrimary : TextBody
                    })
                    {
                        Padding = new Thickness(10, 8, 10, 8),
                        BorderBrush = BorderMuted,
                        BorderThickness = new Thickness(1),
                        Background = r == 0 ? PanelBackground : PageBackground,
                        FontWeight = r == 0 ? FontWeights.SemiBold : FontWeights.Normal
                    });
                }
            }

            return table;
        }

        private Block CreateCodeBlock(IEnumerable<string> lines)
        {
            return new Section(new Paragraph(new Run(string.Join(Environment.NewLine, lines)))
            {
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 13,
                Foreground = TextPrimary,
                Margin = new Thickness(0),
                LineHeight = 20
            })
            {
                Background = CodeBackground,
                BorderBrush = BorderMuted,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(14),
                Margin = new Thickness(0, 6, 0, 16)
            };
        }

        private Block CreateDivider()
        {
            return new Paragraph(new Run(" "))
            {
                BorderBrush = BorderMuted,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Margin = new Thickness(0, 18, 0, 18),
                LineHeight = 1
            };
        }

        private static bool IsNumberedLine(string line)
        {
            var dot = line.IndexOf('.');
            return dot > 0 &&
                   dot < 4 &&
                   line[..dot].All(char.IsDigit) &&
                   line.Length > dot + 1 &&
                   line[dot + 1] == ' ';
        }

        private static string StripNumberPrefix(string line)
        {
            var dot = line.IndexOf('.');
            return dot > -1 && dot + 1 < line.Length
                ? line[(dot + 1)..].Trim()
                : line;
        }

        private static string CleanInlineMarkdown(string text)
        {
            return text
                .Replace("**", string.Empty)
                .Replace("`", string.Empty)
                .Trim();
        }

        private static SolidColorBrush Brush(string hex)
        {
            return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex)!);
        }


        private static string NormalizeLicenseMarkdown(string content)
        {
            var lines = content.Replace("\r\n", "\n").Split('\n');

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (System.Text.RegularExpressions.Regex.IsMatch(line, @"^\d+\.\s+[A-Z].+$"))
                {
                    lines[i] = "## " + line;
                }
            }

            if (!content.TrimStart().StartsWith("#"))
            {
                lines[0] = "# " + lines[0].Trim();
            }

            return string.Join("\n", lines);
        }
        private void ShowAbout()
        {
            DocumentPanel.Visibility = Visibility.Collapsed;
            AboutPanel.Visibility = Visibility.Visible;
        }

        private void LoadLogo()
        {
            var logoPath = Path.Combine(
                AppContext.BaseDirectory,
                "docs",
                "assets",
                "branding",
                "mse_logo.png");

            if (!File.Exists(logoPath))
            {
                AboutLogo.Visibility = Visibility.Collapsed;
                return;
            }

            AboutLogo.Source = new BitmapImage(new Uri(logoPath));
        }


        private void ViewLicenseButton_Click(object sender, RoutedEventArgs e)
        {
            AboutPanel.Visibility = Visibility.Collapsed;
            DocumentPanel.Visibility = Visibility.Visible;

            var content = _helpContentService.LoadDocument("LICENSE.md");
            content = NormalizeLicenseMarkdown(content);
            DocumentViewer.Document = BuildProfessionalDocument(content);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (DocumentViewer.Document?.Blocks.FirstBlock is not null)
                {
                    DocumentViewer.Document.Blocks.FirstBlock.BringIntoView();
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void WebsiteLink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://msetech.org",
                    UseShellExecute = true
                });
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}








