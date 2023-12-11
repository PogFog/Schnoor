using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using IOPath = System.IO.Path;
using static System.Net.Mime.MediaTypeNames;

namespace Chu_ky_Schnorr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private Taokhoa khoa;
        OpenFileDialog openFileDialogSign;
        private bool checkValidInput()
        {
            if (string.IsNullOrEmpty(txtP.Text.Trim()) || string.IsNullOrEmpty(txtQ.Text.Trim()))
            {
                MessageBox.Show("p và q không được bỏ trống!", "Thông báo");
                return false;
            }
            if (string.IsNullOrEmpty(txtG.Text.Trim()))
            {
                MessageBox.Show("g không được bỏ trống!", "Thông báo");
                return false;
            }
            BigInteger p = BigInteger.Parse(txtP.Text);
            BigInteger q = BigInteger.Parse(txtQ.Text);
            if (!ToanHoc.IsPrime(p) || !ToanHoc.IsPrime(q))
            {
                MessageBox.Show("p và q phải là số nguyên tố!", "Thông báo");
                return false;
            }
            
            return true;
        }

        private void NgauNhienK()
        {
            BigInteger q = BigInteger.Parse(txtQ.Text);
            var k = ToanHoc.RandomBigInteger(BigInteger.One, q - 1);
            txtK.Text = k.ToString();
        }
        private void btnChonFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                txtVanBan.Text = fileName;
                using (Stream stream = new FileStream(fileName, FileMode.Open))
                {
                    SHA256Managed sha256Managed = new SHA256Managed();
                    byte[] hashBytes = sha256Managed.ComputeHash(stream);
                    string hashBase64 = Convert.ToBase64String(hashBytes);
                    txtHashText.Text = hashBase64.ToString();
                }
            }

        }

        private void btnFileKtra_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                txtFileKtra.Text = fileName;
                string pathSignatureCheckAuto = IOPath.ChangeExtension(fileName, "sig");
                if (File.Exists(pathSignatureCheckAuto))
                {
                    openFileDialogSign = new OpenFileDialog();
                    openFileDialogSign.FileName = pathSignatureCheckAuto;
                    txtFileChuky.Text = pathSignatureCheckAuto;
                }
            }
        }

        private void btnFileChuKy_Click(object sender, RoutedEventArgs e)
        {
            openFileDialogSign = new OpenFileDialog();
            if (openFileDialogSign.ShowDialog() == true)
                txtFileChuky.Text = openFileDialogSign.FileName;
        }

        private void btnKhoaNgauNhien_Click(object sender, RoutedEventArgs e)
        {
            var random = Taokhoa.TaoKhoaNgauNhien();
            txtQ.Text = random.khoaCongKhai.q.ToString();
            txtP.Text = random.khoaCongKhai.p.ToString();
            txtG.Text = random.khoaCongKhai.g.ToString();
            txtY.Text = random.khoaCongKhai.y.ToString();
            txtX.Text = random.khoaBiMat.x.ToString();
            NgauNhienK();
        }

        private void txtY_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            // Kiểm tra và chắc chắn rằng đoạn văn bản trong TextBox không rỗng
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                // Copy đoạn văn bản trong TextBox vào clipboard
                Clipboard.SetText(textBox.Text);
            }
        }

        private void btnKhoaThuCong_Click(object sender, RoutedEventArgs e)
        {
            if (checkValidInput() == false)
                return;
            BigInteger p = BigInteger.Parse(txtP.Text);
            BigInteger q = BigInteger.Parse(txtQ.Text);
            BigInteger x = 0;
            if (!string.IsNullOrEmpty(txtX.Text.Trim()))
                x = BigInteger.Parse(txtX.Text);
            var handmade = Taokhoa.TaoKhoaThuCong(p, q, x);
            txtY.Text = handmade.khoaCongKhai.y.ToString();
            txtG.Text = handmade.khoaCongKhai.g.ToString();
            txtX.Text = handmade.khoaBiMat.x.ToString();
            NgauNhienK();
        }

        private void btnChonK_Click(object sender, RoutedEventArgs e)
        {
            if (checkValidInput() == false)
                return;
            NgauNhienK();
        }

        private void btnTaoChuKy_Click(object sender, RoutedEventArgs e)
        {
            if (checkValidInput() == false)
                return;
            if (string.IsNullOrEmpty(txtHashText.Text.Trim()))
            {
                MessageBox.Show("Chưa chọn file!", "Thông báo");
                return;
            }
            BigInteger g = BigInteger.Parse(txtG.Text);
            BigInteger k = BigInteger.Parse(txtK.Text);
            BigInteger p = BigInteger.Parse(txtP.Text);
            BigInteger q = BigInteger.Parse(txtQ.Text);
            BigInteger x = BigInteger.Parse(txtX.Text);
            BigInteger y = BigInteger.Parse(txtY.Text);
            khoa = new Taokhoa(p, q, g, x, y);
            BigInteger r = BigInteger.ModPow(g, k, p) % q ;
            string hashBase64 = txtHashText.Text;
            string message = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string fileName = txtVanBan.Text;
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                // Đọc từng dòng trong tệp tin cho đến khi kết thúc
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    message += line+ "\n";
                }
            }
            BigInteger ehash = ToanHoc.Hash(r, message);
            BigInteger kInverse = ToanHoc.ModInverse(k, q);
            BigInteger s = ((ehash + x * r) * kInverse) % q;
            txtS.Text = s.ToString();
            txtR.Text = r.ToString();
        }
        
        private void btnKyVanBan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtR.Text) || string.IsNullOrEmpty(txtS.Text))
            {
                MessageBox.Show("Chưa tạo chữ ký!", "Thông báo");
                return;
            }
            try
            {
                string path = IOPath.ChangeExtension(txtVanBan.Text, "sig");
                if (File.Exists(path))
                {
                    if (MessageBox.Show("File: " + IOPath.GetFileName(path) + " đã có. Có muốn ghi đè?", "File đã có", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                using (StreamWriter streamWrite = File.CreateText(path))
                {
                    streamWrite.WriteLine(txtR.Text);
                    streamWrite.WriteLine(txtS.Text);
                }
                MessageBox.Show("Ký văn bản thành công", "Thông báo");
            }
            catch
            {
                MessageBox.Show("Lỗi", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnKtraChuKy_Click(object sender, RoutedEventArgs e)
        {
            if(checkValidInput() == false) return;
            if (string.IsNullOrEmpty(txtFileChuky.Text.Trim()) ||
                string.IsNullOrEmpty(txtFileKtra.Text.Trim()))
            {
                MessageBox.Show("Chưa chọn file kiểm tra!", "Thông báo");
                return;
            }
            if (string.IsNullOrEmpty(txtYInput.Text.Trim()))
            {
                MessageBox.Show("Chưa nhập khóa công khai y!", "Thông báo");
                return;
            }
            try
            {
                BigInteger r, s;
                string hashBase64;
                using (Stream stream = new FileStream(txtFileKtra.Text, FileMode.Open))
                {
                    SHA256Managed sha256Managed = new SHA256Managed();
                    byte[] hashBytes = sha256Managed.ComputeHash(stream);
                    hashBase64 = Convert.ToBase64String(hashBytes);
                }
                using (StreamReader streamSig = new StreamReader(txtFileChuky.Text))
                {
                    r = BigInteger.Parse(streamSig.ReadLine());
                    s = BigInteger.Parse(streamSig.ReadLine());
                }

                var hashFromBase64 = Convert.FromBase64String(hashBase64);
                var hmValue = new BigInteger(hashFromBase64.Concat(new byte[] { 0x00 }).ToArray());
                BigInteger q = khoa.khoaCongKhai.q;
                BigInteger p = khoa.khoaCongKhai.p;
                BigInteger g = khoa.khoaCongKhai.g;
                BigInteger y = BigInteger.Parse(txtYInput.Text);

                string message = "";
                OpenFileDialog openFileDialog = new OpenFileDialog();
                string fileName = txtFileKtra.Text;
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    // Đọc từng dòng trong tệp tin cho đến khi kết thúc
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        message += line + "\n";
                    }
                }

                BigInteger ehash = ToanHoc.Hash(r, message);
                BigInteger w = ToanHoc.ModInverse(s, q);
                BigInteger u1 = (w * ehash) % q;
                BigInteger u2 = (r * w) % q;
                BigInteger gU1 = BigInteger.ModPow(g, u1, p);
                BigInteger yU2 = BigInteger.ModPow(y, u2, p);
                BigInteger v = ((gU1 * yU2) % p) % q;
                if (r != BigInteger.Parse(txtR.Text) || s != BigInteger.Parse(txtS.Text) || y != BigInteger.Parse(txtY.Text)){
                    MessageBox.Show("Chữ kí không hợp lệ !");
                    return;
                }
                else {
                    bool isValid = (v == r);

                    if (isValid)
                    {
                        MessageBox.Show("Văn bản chưa bị chỉnh sửa!", "Thông báo");
                        MessageBox.Show("Chữ kí hợp lệ", "Thông báo");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Văn bản đã bị chỉnh sửa !", "Thông báo");
                        return;
                    }
                }
            }catch(Exception) 
            {
                MessageBox.Show("Lỗi không đọc được file!", "Lỗi");
                return;
            }

        }
        private void btnXoatrango_Click(object sender, RoutedEventArgs e)
        {
            txtQ.Clear();
            txtP.Clear();
            txtG.Clear();
            txtX.Clear();
            txtY.Clear();
            txtK.Clear();
        }
    }
}

