using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Roulette_GUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Game : Page
    {
        public readonly Random random = new Random();
        public List<string> PROPS = new List<string>()
        {
            "Magnifier",//Check bullet
	        "Saw",//Double hurt,20% boom(boom takes 2 lives)
	        "Rope",//Limit action
	        "Stimulant",//1 life cost,3 bullet shot
	        "Converter",//Change the bullet
	        "Bandage",//1 life +
	        "Beer",//1 bullet thrown
	        "Hook"//Take the others 1 prop
        };
        public List<bool> bullets = new List<bool>();
        List<string> props_1 = new List<string>();
        List<string> props_2 = new List<string>();
        List<short> life = new List<short>() { 0, 0 };
        bool sawed_1 = false;
        bool sawed_2 = false;
        bool stimulanted_1 = false;
        bool stimulanted_2 = false;
        bool roped_1 = false;
        bool roped_2 = false;
        bool opreating_player = true;
        int heart_num = 0;
        public void NewImage(Panel parent, string Name, string ImgName, int width)
        {
            Image img = new Image();
            parent.Children.Add(img);
            BitmapImage bitmapImage = new BitmapImage();
            img.Width = bitmapImage.DecodePixelWidth = width;
            bitmapImage.UriSource = new Uri(img.BaseUri, "./Assets/"+ImgName+".png");
            img.Source = bitmapImage;
            img.Name=Name;
            Thickness _margin = new Thickness() { };
        }
        public void ReplaceImage(string Name, string ImgName, int width)
        {
            Image img = FindName(Name) as Image;
            BitmapImage bitmapImage = new BitmapImage();
            img.Width = bitmapImage.DecodePixelWidth = width;
            bitmapImage.UriSource = new Uri(img.BaseUri, "./Assets/"+ImgName+".png");
            img.Source = bitmapImage;
        }
        public async void Fill()
        {
            if (life[0] > 0 && life[1] > 0)
            {
                //Bullet
                for (int i = 0; i < random.Next(4, 11); i++)
                {
                    bullets.Add(random.Next(2) == 1);
                    NewImage(Bullets, "bullet_"+(i+1).ToString()+"_I", "bullet_unknown", 100);
                    await Task.Delay(100);
                }
                //Prop
                int prop_num = random.Next(2, 5);
                for (int i = 0; i < prop_num; i++)
                {
                    string prop = PROPS[random.Next(PROPS.Count)];
                    props_1.Add(prop);
                    Prop_1_LV.Items.Add(new TextBlock() { FontFamily=new FontFamily("Monocraft Nerd Font"), FontSize=20, Text=prop });
                    await Task.Delay(100);
                }
                for (int i = 0; i < prop_num; i++)
                {
                    string prop = PROPS[random.Next(PROPS.Count)];
                    props_2.Add(prop);
                    Prop_2_LV.Items.Add(new TextBlock() { FontFamily=new FontFamily("Monocraft Nerd Font"), FontSize=20, Text=prop });
                    await Task.Delay(100);
                }
                int live_num = 0;
                int blank_num = 0;
                for (int i = 0; i<bullets.Count; i++)
                {
                    if (bullets[i])
                    {
                        live_num++;
                    }
                    else
                    {
                        blank_num++;
                    }

                }
                ContentDialog BulletDialog = new ContentDialog()
                {
                    Title = "Bullet Filled",
                    Content = "Bullet Filled.\n"+live_num.ToString()+" Live Round(s).\n"+blank_num.ToString()+" Blank Round(s).",
                    PrimaryButtonText = "OK"
                };

                await BulletDialog.ShowAsync();
            }
        }
        public async void Init(object sender, RoutedEventArgs e)
        {
            //Heart
            heart_num = random.Next(4, 11);
            for (int i = 0; i < heart_num; i++)
            {
                life[0]++;
                NewImage(Lives_1, "life_1_"+(i+1).ToString()+"_I", "heart", 50);
                await Task.Delay(100);
                life[1]++;
                NewImage(Lives_2, "life_2_"+(i+1).ToString()+"_I", "heart", 50);
                await Task.Delay(100);
            }
            Fill();
            Turn();
        }
        public async void IsFinished()
        {
            if (life[0]<=0)
            {
                ContentDialog FinishDialog = new ContentDialog()
                {
                    Title = "Player 2 Won!",
                    Content = "Player 2 Won!",
                    PrimaryButtonText = "Next Round"
                };

                await FinishDialog.ShowAsync();
                Frame.GoBack();
                Frame.GoForward();
            }
            if (life[1]<=0)
            {
                ContentDialog FinishDialog = new ContentDialog()
                {
                    Title = "Player 1 Won!",
                    Content = "Player 1 Won!",
                    PrimaryButtonText = "Next Round",
                };

                await FinishDialog.ShowAsync();
                Frame.GoBack();
                Frame.GoForward();
            }
        }
        public void Turn()
        {
            (FindName("Prop_"+(opreating_player ? 2 : 1).ToString()+"_B") as Button).IsEnabled=false;
            (FindName("Target_"+(opreating_player ? 2 : 1).ToString()+"_RB") as RadioButtons).IsEnabled=false;
            (FindName("Shoot_"+(opreating_player ? 2 : 1).ToString()+"_B") as Button).IsEnabled=false;
            (FindName("Prop_"+(!opreating_player ? 2 : 1).ToString()+"_B") as Button).IsEnabled=true;
            (FindName("Target_"+(!opreating_player ? 2 : 1).ToString()+"_RB") as RadioButtons).IsEnabled=true;
            (FindName("Shoot_"+(!opreating_player ? 2 : 1).ToString()+"_B") as Button).IsEnabled=true;
            opreating_player = !opreating_player;
        }
        private async void Shoot_1(object sender, RoutedEventArgs e)
        {
            if (bullets.Count >= 1)
            {
                //Bullets.Children.ElementAt(Bullets.Children.IndexOf(FindName("bullet_"+(bullets.Count).ToString()) as Image)).SetValue("./Assets/bullet_unknown.png",);
                if (stimulanted_1)
                {
                    stimulanted_1 = false;
                    int bullet_num = 0;
                    while (bullets.Count >= 1)
                    {
                        bullet_num++;
                        if (bullet_num > 3)
                        {
                            break;
                        }
                        ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                        await Task.Delay(500);
                        if (bullets[bullets.Count - 1])
                        {//live round
                            if (sawed_1)
                            {//sawed
                                if (random.Next(1, 6) != 1)
                                {//not boom
                                    ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1).ToString()+"_"+life[Target_1_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                    life[Target_1_RB.SelectedIndex]--;
                                    IsFinished();
                                    ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1).ToString()+"_"+life[Target_1_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                    life[Target_1_RB.SelectedIndex]--;
                                    IsFinished();
                                }
                                else
                                {//boom
                                    ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1 == 1 ? 2 : 1).ToString()+"_"+life[Target_1_RB.SelectedIndex == 1 ? 0 : 1].ToString()+"_I", "heart_lost", 50);
                                    life[Target_1_RB.SelectedIndex == 1 ? 0 : 1]--;
                                    IsFinished();
                                    ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1 == 1 ? 2 : 1).ToString()+"_"+life[Target_1_RB.SelectedIndex == 1 ? 0 : 1].ToString()+"_I", "heart_lost", 50);
                                    life[Target_1_RB.SelectedIndex == 1 ? 0 : 1]--;
                                    IsFinished();
                                }
                            }
                            else
                            {//not sawed
                                ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1).ToString()+"_"+life[Target_1_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                life[Target_1_RB.SelectedIndex]--;
                                IsFinished();
                            }

                        }
                        ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", null, 0);
                        bullets.RemoveAt(bullets.Count-1);
                        IsFinished();
                    }
                    if (!roped_2)
                        Turn();
                    roped_2= false;
                }
                else
                {
                    ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                    if (bullets[bullets.Count - 1])
                    {//live round

                        if (sawed_1)
                        {//sawed
                            if (random.Next(1, 6) != 1)
                            {//not boom
                                ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1).ToString()+"_"+life[Target_1_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                life[Target_1_RB.SelectedIndex]--;
                                IsFinished();
                                ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1).ToString()+"_"+life[Target_1_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                life[Target_1_RB.SelectedIndex]--;
                                IsFinished();
                            }
                            else
                            {//boom
                                ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1 == 1 ? 2 : 1).ToString()+"_"+life[Target_1_RB.SelectedIndex == 1 ? 0 : 1].ToString()+"_I", "heart_lost", 50);
                                life[Target_1_RB.SelectedIndex == 1 ? 0 : 1]--;
                                IsFinished();
                                ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1 == 1 ? 2 : 1).ToString()+"_"+life[Target_1_RB.SelectedIndex == 1 ? 0 : 1].ToString()+"_I", "heart_lost", 50);
                                life[Target_1_RB.SelectedIndex == 1 ? 0 : 1]--;
                                IsFinished();
                            }
                        }
                        else
                        {//not sawed
                            ReplaceImage("life_"+(Target_1_RB.SelectedIndex+1).ToString()+"_"+life[Target_1_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                            life[Target_1_RB.SelectedIndex]--;
                            IsFinished();
                        }
                        if (!roped_2)
                            Turn();
                        roped_2= false;
                    }
                    else
                    {
                        if (Target_1_RB.SelectedIndex+1 == 2 && !roped_2)
                        {
                            Turn();
                        }
                        roped_2=false;
                    }
                    await Task.Delay(500);
                    ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", null, 0);
                    bullets.RemoveAt(bullets.Count-1);
                }

                sawed_1=false;


                if (bullets.Count <= 0)
                {
                    await Task.Delay(500);
                    Fill();
                }


            }
        }
        private async void Shoot_2(object sender, RoutedEventArgs e)
        {
            if (bullets.Count >= 1)
            {
                //Bullets.Children.ElementAt(Bullets.Children.IndexOf(FindName("bullet_"+(bullets.Count).ToString()) as Image)).SetValue("./Assets/bullet_unknown.png",);
                if (stimulanted_2)
                {
                    stimulanted_2 = false;
                    int bullet_num = 0;
                    while (bullets.Count >= 1)
                    {
                        bullet_num++;
                        if (bullet_num > 3)
                        {
                            break;
                        }
                        ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                        await Task.Delay(500);
                        if (bullets[bullets.Count - 1])
                        {//live round
                            if (sawed_2)
                            {//sawed
                                if (random.Next(1, 6) != 1)
                                {//not boom
                                    ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1).ToString()+"_"+life[Target_2_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                    life[Target_2_RB.SelectedIndex]--;
                                    IsFinished();
                                    ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1).ToString()+"_"+life[Target_2_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                    life[Target_2_RB.SelectedIndex]--;
                                    IsFinished();
                                }
                                else
                                {//boom
                                    ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1 == 1 ? 2 : 1).ToString()+"_"+life[Target_2_RB.SelectedIndex == 1 ? 0 : 1].ToString()+"_I", "heart_lost", 50);
                                    life[Target_2_RB.SelectedIndex == 1 ? 0 : 1]--;
                                    IsFinished();
                                    ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1 == 1 ? 2 : 1).ToString()+"_"+life[Target_2_RB.SelectedIndex == 1 ? 0 : 1].ToString()+"_I", "heart_lost", 50);
                                    life[Target_2_RB.SelectedIndex == 1 ? 0 : 1]--;
                                    IsFinished();
                                }
                            }
                            else
                            {//not sawed
                                ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1).ToString()+"_"+life[Target_2_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                life[Target_2_RB.SelectedIndex]--;
                                IsFinished();
                            }
                        }
                        ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", null, 0);
                        bullets.RemoveAt(bullets.Count-1);
                    }
                    if (!roped_1)
                        Turn();
                    roped_1= false;
                }
                else
                {
                    ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                    if (bullets[bullets.Count - 1])
                    {//live round

                        if (sawed_2)
                        {//sawed
                            if (random.Next(1, 6) != 1)
                            {//not boom
                                ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1).ToString()+"_"+life[Target_2_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                life[Target_2_RB.SelectedIndex]--;
                                IsFinished();
                                ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1).ToString()+"_"+life[Target_2_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                                life[Target_2_RB.SelectedIndex]--;
                                IsFinished();
                            }
                            else
                            {//boom
                                ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1 == 1 ? 2 : 1).ToString()+"_"+life[Target_2_RB.SelectedIndex == 1 ? 0 : 1].ToString()+"_I", "heart_lost", 50);
                                life[Target_2_RB.SelectedIndex == 1 ? 0 : 1]--;
                                IsFinished();
                                ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1 == 1 ? 2 : 1).ToString()+"_"+life[Target_2_RB.SelectedIndex == 1 ? 0 : 1].ToString()+"_I", "heart_lost", 50);
                                life[Target_2_RB.SelectedIndex == 1 ? 0 : 1]--;
                                IsFinished();
                            }
                        }
                        else
                        {//not sawed
                            ReplaceImage("life_"+(Target_2_RB.SelectedIndex+1).ToString()+"_"+life[Target_2_RB.SelectedIndex].ToString()+"_I", "heart_lost", 50);
                            life[Target_2_RB.SelectedIndex]--;
                            IsFinished();
                        }
                        if (!roped_1)
                            Turn();
                        roped_1= false;
                    }
                    else
                    {
                        if (Target_2_RB.SelectedIndex+1 == 1 && !roped_1)
                        {
                            Turn();
                        }
                        roped_1= false;
                    }

                    await Task.Delay(500);
                    ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", null, 0);
                    bullets.RemoveAt(bullets.Count-1);
                }

                sawed_2=false;
                if (bullets.Count <= 0)
                {
                    await Task.Delay(500);
                    Fill();
                }
            }
        }
        private async void Prop_1(object sender, RoutedEventArgs e)
        {
            int prop_index;
            prop_index =  Prop_1_LV.SelectedIndex;

            string prop = props_1[prop_index];
            if (prop == "Magnifier")
            {
                ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                props_1.RemoveAt(prop_index);
                Prop_1_LV.Items.RemoveAt(prop_index);
            }
            else
            {
                if (prop == "Saw")
                {
                    if (!sawed_1)
                    {
                        sawed_1 = true;
                        props_1.RemoveAt(prop_index);
                        Prop_1_LV.Items.RemoveAt(prop_index);
                    }
                }
                else
                {
                    if (prop == "Rope")
                    {
                        if (!roped_2)
                        {
                            roped_2 = true;
                            props_1.RemoveAt(prop_index);
                            Prop_1_LV.Items.RemoveAt(prop_index);
                        }
                        else
                        {
                            ContentDialog RopeDialog = new ContentDialog()
                            {
                                Title = "Roped",
                                Content = "Already Roped!",
                                PrimaryButtonText = "OK"
                            };
                            await RopeDialog.ShowAsync();
                        }
                    }
                    else
                    {
                        if (prop == "Stimulant")
                        {
                            if (!stimulanted_1)
                            {
                                stimulanted_1 = true;
                                ReplaceImage("life_1_"+life[0].ToString()+"_I", "heart_lost", 50);
                                life[0] -= 1;
                                props_1.RemoveAt(prop_index);
                                Prop_1_LV.Items.RemoveAt(prop_index);
                            }
                            else
                            {
                                ContentDialog StimulantDialog = new ContentDialog()
                                {
                                    Title = "Stimulant Used",
                                    Content = "Already Used Stimulant!",
                                    PrimaryButtonText = "OK"
                                };
                                await StimulantDialog.ShowAsync();
                            }
                        }
                        else
                        {
                            if (prop == "Converter")
                            {
                                bullets[bullets.Count - 1] = !bullets[bullets.Count - 1];
                                if (((Bullets.Children.ElementAt(bullets.Count-1) as Image).Source as BitmapImage).UriSource.AbsolutePath!="/Assets/bullet_unknown.png"
                                    &&((Bullets.Children.ElementAt(bullets.Count-1) as Image).Source as BitmapImage).UriSource.AbsolutePath!="/Assets/.png")

                                {
                                    ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                                }
                                props_1.RemoveAt(prop_index);
                                Prop_1_LV.Items.RemoveAt(prop_index);
                            }
                            else
                            {
                                if (prop == "Bandage")
                                {
                                    if (life[0] < heart_num)
                                    {
                                        life[0] += 1;
                                        ReplaceImage("life_1_"+life[0].ToString()+"_I", "heart", 50);
                                        props_1.RemoveAt(prop_index);
                                        Prop_1_LV.Items.RemoveAt(prop_index);
                                    }
                                    else
                                    {
                                        ContentDialog BandageDialog = new ContentDialog()
                                        {
                                            Title = "Heart Full",
                                            Content = "Heart Full!",
                                            PrimaryButtonText = "OK"
                                        };
                                        await BandageDialog.ShowAsync();
                                    }

                                }
                                else
                                {
                                    if (prop == "Beer")
                                    {
                                        ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                                        await Task.Delay(500);
                                        ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", null, 0);
                                        bullets.RemoveAt(bullets.Count-1);
                                        props_1.RemoveAt(prop_index);
                                        Prop_1_LV.Items.RemoveAt(prop_index);
                                        if (bullets.Count<=0)
                                        {
                                            Fill();
                                        }
                                    }
                                    else
                                    {
                                        if (prop == "Hook")
                                        {

                                            int hook_prop_index = Prop_2_LV.SelectedIndex;
                                            if (props_2[hook_prop_index]=="Hook")
                                            {
                                                ContentDialog HookDialog = new ContentDialog()
                                                {
                                                    Title = "Don't Hook Hook",
                                                    Content = "No Hooking Hook!",
                                                    PrimaryButtonText = "OK"
                                                };
                                                await HookDialog.ShowAsync();
                                            }
                                            else
                                            {
                                                props_1.RemoveAt(prop_index);
                                                Prop_1_LV.Items.RemoveAt(prop_index);
                                                Prop_2_LV.Items.RemoveAt(hook_prop_index);
                                                Prop_1_LV.Items.Add(new TextBlock() { FontFamily=new FontFamily("Monocraft Nerd Font"), FontSize=20, Text=props_2[hook_prop_index] });
                                                props_1.Add(props_2[hook_prop_index]);
                                                props_2.RemoveAt(hook_prop_index);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private async void Prop_2(object sender, RoutedEventArgs e)
        {
            int prop_index;
            prop_index =  Prop_2_LV.SelectedIndex;

            string prop = props_2[prop_index];
            if (prop == "Magnifier")
            {
                ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                props_2.RemoveAt(prop_index);
                Prop_2_LV.Items.RemoveAt(prop_index);
            }
            else
            {
                if (prop == "Saw")
                {
                    if (!sawed_2)
                    {
                        sawed_2 = true;
                        props_2.RemoveAt(prop_index);
                        Prop_2_LV.Items.RemoveAt(prop_index);
                    }
                }
                else
                {
                    if (prop == "Rope")
                    {
                        if (!roped_1)
                        {
                            roped_1 = true;
                            props_2.RemoveAt(prop_index);
                            Prop_2_LV.Items.RemoveAt(prop_index);
                        }
                        else
                        {
                            ContentDialog RopeDialog = new ContentDialog()
                            {
                                Title = "Roped",
                                Content = "Already Roped!",
                                PrimaryButtonText = "OK"
                            };
                            await RopeDialog.ShowAsync();
                        }
                    }
                    else

                        if (prop == "Stimulant")
                    {
                        if (!stimulanted_2)
                        {
                            stimulanted_2 = true;
                            ReplaceImage("life_2_"+life[1].ToString()+"_I", "heart_lost", 50);
                            life[1] -= 1;
                            props_2.RemoveAt(prop_index);
                            Prop_2_LV.Items.RemoveAt(prop_index);
                        }
                        else
                        {
                            ContentDialog StimulantDialog = new ContentDialog()
                            {
                                Title = "Stimulant Used",
                                Content = "Already Used Stimulant!",
                                PrimaryButtonText = "OK"
                            };
                            await StimulantDialog.ShowAsync();
                        }
                    }
                    else
                    {
                        if (prop == "Converter")
                        {
                            bullets[bullets.Count - 1] = !bullets[bullets.Count - 1];
                            if (((Bullets.Children.ElementAt(bullets.Count-1) as Image).Source as BitmapImage).UriSource.AbsolutePath!="/Assets/bullet_unknown.png"
                                &&((Bullets.Children.ElementAt(bullets.Count-1) as Image).Source as BitmapImage).UriSource.AbsolutePath!="/Assets/.png")
                            {
                                ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                            }
                            props_2.RemoveAt(prop_index);
                            Prop_2_LV.Items.RemoveAt(prop_index);
                        }
                        else
                        {
                            if (prop == "Bandage")
                            {
                                if (life[1] < heart_num)
                                {
                                    life[1] += 1;
                                    ReplaceImage("life_2_"+life[1].ToString()+"_I", "heart", 50);
                                    props_2.RemoveAt(prop_index);
                                    Prop_2_LV.Items.RemoveAt(prop_index);
                                }
                                else
                                {
                                    ContentDialog BandageDialog = new ContentDialog()
                                    {
                                        Title = "Heart Full",
                                        Content = "Heart Full!",
                                        PrimaryButtonText = "OK"
                                    };
                                    await BandageDialog.ShowAsync();
                                }

                            }
                            else
                            {
                                if (prop == "Beer")
                                {
                                    ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", bullets[bullets.Count-1] ? "bullet_live" : "bullet_blank", 100);
                                    await Task.Delay(500);
                                    ReplaceImage("bullet_"+(bullets.Count).ToString()+"_I", null, 0);
                                    bullets.RemoveAt(bullets.Count-1);
                                    props_2.RemoveAt(prop_index);
                                    Prop_2_LV.Items.RemoveAt(prop_index);
                                    if (bullets.Count<=0)
                                    {
                                        Fill();
                                    }
                                }
                                else
                                {
                                    if (prop == "Hook")
                                    {

                                        int hook_prop_index = Prop_1_LV.SelectedIndex;
                                        if (props_1[hook_prop_index]=="Hook")
                                        {
                                            ContentDialog HookDialog = new ContentDialog()
                                            {
                                                Title = "Don't Hook Hook",
                                                Content = "No Hooking Hook!",
                                                PrimaryButtonText = "OK"
                                            };
                                            await HookDialog.ShowAsync();
                                        }
                                        else
                                        {
                                            props_2.RemoveAt(prop_index);
                                            Prop_2_LV.Items.RemoveAt(prop_index);
                                            Prop_1_LV.Items.RemoveAt(hook_prop_index);
                                            Prop_2_LV.Items.Add(new TextBlock() { FontFamily=new FontFamily("Monocraft Nerd Font"), FontSize=20, Text=props_1[hook_prop_index] });
                                            props_2.Add(props_1[hook_prop_index]);
                                            props_1.RemoveAt(hook_prop_index);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public Game()
        {
            this.InitializeComponent();
            Loaded += Init;
        }
    }
}
