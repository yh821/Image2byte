using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PictureToByte
{
    public partial class Form1 : Form
    {
        private byte[] mBuffer;
        private Size mBufferSize;
        private string FileName;

        /// <summary>
        /// 打开图片
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Image|*.png;*.jpg;*.bmp";
            of.Title = "图片路径";

            if (of.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(of.FileName))
            {
                try
                {
                    FileName = Path.GetFileName(of.FileName);
                    FileName = FileName.Substring(0, FileName.LastIndexOf('.'));
                    Image im = Image.FromFile(of.FileName);
                    pictureBox1.Image = im;
                    mBufferSize = im.Size;
                    label1.Text = $"尺寸:{mBufferSize.Width}x{mBufferSize.Height}";
                    label2.Text = $"类型:{ImageFormatToString(pictureBox1.Image)}";

                    MemoryStream ms = new MemoryStream();
                    im.Save(ms, im.RawFormat);
                    mBuffer = ms.GetBuffer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "打开图片失败!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// 导出byte
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("未导入图片！");
                return;
            }

            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < mBuffer.Length; i++)
                {
                    if (checkBox1.Checked)
                    {
                        if (i == 0)
                            sb.Append(mBuffer[i].ToString("D3"));
                        else if (i % mBufferSize.Width == 0)
                            sb.Append(',').Append("\r\n").Append(mBuffer[i].ToString("D3"));
                        else
                            sb.Append(',').Append(mBuffer[i].ToString("D3"));
                    }
                    else
                    {
                        if (i == 0)
                            sb.Append(mBuffer[i]);
                        else if (i % mBufferSize.Width == 0)
                            sb.Append(',').Append("\r\n").Append(mBuffer[i]);
                        else
                            sb.Append(',').Append(mBuffer[i]);
                    }
                }

                string path = $"{Application.StartupPath}\\txt";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = $"{path}\\{FileName}_byte.txt";

                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(sb.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "导出txt文本失败!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// 导入byte
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Text Document(*.txt)|*.txt";
            of.Title = "选择要导入的txt文本路径";

            if (of.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(of.FileName))
            {
                try
                {
                    FileName = Path.GetFileName(of.FileName);
                    FileName = FileName.Substring(0, FileName.LastIndexOf('.'));
                    StreamReader sr = new StreamReader(of.FileName, System.Text.Encoding.UTF8, true, 128);
                    string[] strBytes = sr.ReadToEnd().Split(',');
                    sr.Close();
                    byte[] bytes = new byte[strBytes.Length];
                    for (int i = 0; i < bytes.Length; i++)
                        bytes[i] = (byte)int.Parse(strBytes[i]);
                    mBuffer = bytes;
                    MemoryStream ms = new MemoryStream(bytes);
                    pictureBox1.Image = new Bitmap(ms);
                    mBufferSize = pictureBox1.Image.Size;
                    label1.Text = $"尺寸:{mBufferSize.Width}x{mBufferSize.Height}";
                    label2.Text = $"类型:{ImageFormatToString(pictureBox1.Image)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导入txt文本失败!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// 导出base64
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("未导入图片！");
                return;
            }

            try
            {
                string path = $"{Application.StartupPath}\\txt";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = $"{path}\\{FileName}_base64.txt";

                StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8, 128);
                sw.WriteLine(Convert.ToBase64String(mBuffer));
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "导出txt文本失败!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// 导入base64
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Text Document(*.txt)|*.txt";
            of.Title = "选择要导入的txt文本路径";

            if (of.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(of.FileName))
            {
                try
                {
                    FileName = Path.GetFileName(of.FileName);
                    FileName = FileName.Substring(0, FileName.LastIndexOf('.'));
                    StreamReader sr = new StreamReader(of.FileName, Encoding.UTF8, true, 128);
                    string utf8 = sr.ReadToEnd();
                    sr.Close();
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(utf8));
                    pictureBox1.Image = new Bitmap(ms);
                    mBufferSize = pictureBox1.Image.Size;
                    label1.Text = $"尺寸:{mBufferSize.Width}x{mBufferSize.Height}";
                    label2.Text = $"类型:{ImageFormatToString(pictureBox1.Image)}";

                    ms = new MemoryStream();
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    mBuffer = ms.GetBuffer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导入txt文本失败!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("未导入图片！");
                return;
            }
            try
            {
                string path = $"{Application.StartupPath}\\img";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                pictureBox1.Image.Save($"{path}\\{FileName}.{ImageFormatToString(pictureBox1.Image)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "保存图片失败!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private string ImageFormatToString(Image image)
        {
            if (image.RawFormat.Guid == ImageFormat.Tiff.Guid)
                return "tiff";
            if (image.RawFormat.Guid == ImageFormat.Gif.Guid)
                return "gif";
            if (image.RawFormat.Guid == ImageFormat.Jpeg.Guid)
                return "jpg";
            if (image.RawFormat.Guid == ImageFormat.Bmp.Guid)
                return "bmp";
            if (image.RawFormat.Guid == ImageFormat.Png.Guid)
                return "png";
            if (image.RawFormat.Guid == ImageFormat.Icon.Guid)
                return "icon";

            return "???";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("未导入图片！");
                return;
            }

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Png Image|*.png|Jpeg Image|*.jpg|Bmp Image|*.bmp";
            sf.Title = "选择保存的图片路径";

            if (sf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileStream fs = (FileStream)sf.OpenFile();
                    switch (sf.FilterIndex)
                    {
                        case 1:
                            pictureBox1.Image.Save(fs, ImageFormat.Png);
                            break;

                        case 2:
                            pictureBox1.Image.Save(fs, ImageFormat.Jpeg);
                            break;

                        case 3:
                            pictureBox1.Image.Save(fs, ImageFormat.Bmp);
                            break;
                    }
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "保存图片失败!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("未导入图片！");
                return;
            }

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text Document(*.txt)|*.txt";
            sf.Title = "选择保存的txt文本路径";

            if (sf.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sf.FileName))
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < mBuffer.Length; i++)
                    {
                        if (checkBox1.Checked)
                        {
                            if (i == 0)
                                sb.Append(mBuffer[i].ToString("D3"));
                            else if (i % mBufferSize.Width == 0)
                                sb.Append("\r\n").Append(mBuffer[i].ToString("D3"));
                            else
                                sb.Append(',').Append(mBuffer[i].ToString("D3"));
                        }
                        else
                        {
                            if (i == 0)
                                sb.Append(mBuffer[i]);
                            else if (i % mBufferSize.Width == 0)
                                sb.Append("\r\n").Append(mBuffer[i]);
                            else
                                sb.Append(',').Append(mBuffer[i]);
                        }
                    }

                    StreamWriter sw = new StreamWriter(sf.FileName);
                    sw.WriteLine(sb.ToString());
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导出txt文本失败!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("未导入图片！");
                return;
            }

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text Document(*.txt)|*.txt";
            sf.Title = "选择保存的txt文本路径";

            if (sf.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sf.FileName))
            {
                try
                {
                    StreamWriter sw = new StreamWriter(sf.FileName, false, Encoding.UTF8, 128);
                    sw.WriteLine(Convert.ToBase64String(mBuffer));
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导出txt文本失败!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string path = $"{Application.StartupPath}\\txt";
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string path = $"{Application.StartupPath}\\img";
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }
    }
}
