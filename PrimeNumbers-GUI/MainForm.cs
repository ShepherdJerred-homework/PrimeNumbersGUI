using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrimeNumbers_GUI {
    public partial class MainForm : Form {
        private CancellationTokenSource cancellationTokenSource;

        public MainForm() {
            InitializeComponent();
        }

        private async void startButton_Click(object sender, EventArgs e) {
            // Find all prime numbers starting between the first and last numbers
            int firstNum;
            int lastNum;
            try {
                firstNum = Convert.ToInt32(startNumTextBox.Text);
                lastNum = Convert.ToInt32(endNumTextBox.Text);
            } catch (FormatException) {
                MessageBox.Show("Invalid input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            numbersTextBox.Clear();

            // Prevent user from messing with certain controls while job is running
            progressBar1.Minimum = firstNum;
            progressBar1.Maximum = lastNum;
            progressBar1.Visible = true;
            cancelButton.Enabled = true;
            pauseButton.Enabled = true;
            startNumTextBox.Enabled = false;
            endNumTextBox.Enabled = false;

            UseWaitCursor = true;

            Task findPrimeTask = FindAndPrintPrimes(firstNum, lastNum);
            await findPrimeTask;

            // Let the user know we did something even if no prime nums were found
            if (numbersTextBox.TextLength == 0) {
                numbersTextBox.Text = "None.";
            }

            UseWaitCursor = false;

            // Reset the form
            startNumTextBox.Enabled = true;
            endNumTextBox.Enabled = true;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.Visible = false;
            cancelButton.Enabled = false;
            pauseButton.Enabled = false;
        }

        private async Task FindAndPrintPrimes(int firstNum, int lastNum) {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            await Task.Run(() => {
                for (int i = firstNum; i <= lastNum; i++) {
                    if (token.IsCancellationRequested) {
                        break;
                    }
                    if (IsPrime(i)) {
                        AddNumberToTextBox(i);
                    }
                }
            });
        }

        private bool IsPrime(int num) {
            if (num < 2)
                return false;

            // Look for a number that evenly divides the num
            for (int i = 2; i <= num / 2; i++)
                if (num % i == 0)
                    return false;

            // No divisors means the number is prime
            return true;
        }

        private void AddNumberToTextBox(int num) {
            Invoke((Action) delegate() {
                numbersTextBox.AppendText(num + "\n");
                progressBar1.Value = num;
            });
        }

        private void pauseButton_Click(object sender, EventArgs e) {
            cancellationTokenSource.Cancel();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            cancellationTokenSource.Cancel();
        }
    }
}