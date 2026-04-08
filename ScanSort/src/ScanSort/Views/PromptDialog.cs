namespace ScanSort.Views;

public static class PromptDialog
{
    public static string Show(string promptText, string caption)
    {
        using var form = new Form
        {
            Width = 500,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = caption,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var label = new Label { Left = 50, Top = 20, Text = promptText, AutoSize = true };
        var textBox = new TextBox { Left = 50, Top = 50, Width = 400 };
        var button = new Button
        {
            Text = "OK",
            Left = 350,
            Width = 100,
            Top = 70,
            DialogResult = DialogResult.OK
        };
        button.Click += (s, e) => form.Close();

        form.Controls.Add(label);
        form.Controls.Add(textBox);
        form.Controls.Add(button);
        form.AcceptButton = button;

        return form.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
    }
}
