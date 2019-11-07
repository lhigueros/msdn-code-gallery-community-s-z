# WPF FlashMessage Control
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- WPF
## Topics
- Controls
- Animation
- WPF CustomControl
- Fading Effect
## Updated
- 04/19/2012
## Description

<h1>The Motivation</h1>
<p><em>&nbsp; &nbsp; &nbsp;Aren't you tired of all the MessageBoxes on your applications? Isn't there a better way to alert users whether an operation was successfully completed? For years I used MessageBox or a static label in WinForms applications, and they
 came with me when I started to migrate to WPF.</em></p>
<p><em>&nbsp; &nbsp; &nbsp;However,&nbsp;WPF is so much more powerfull than that, and I thought why not create something different? Because let's face it, using MessageBox is like using javascript alert function in a web page, don't you think?. So it is settle,
 we need something new, but what? Well, once I was a <a title="RubyOnRails" href="http://rubyonrails.org/" target="_blank">
Rails</a> developer and Rails has a interesting <a title="The Flash" href="http://guides.rubyonrails.org/action_controller_overview.html#the-flash" target="_blank">
flash message system</a>, so I thought of creating a control that acts slightly&nbsp;similar.</em></p>
<h1>How does it work?</h1>
<p><em>&nbsp; &nbsp; &nbsp;So, let's see how the control works. It has two mainly properties: MessageType and Message.</em></p>
<p><em>&nbsp; &nbsp; &nbsp;The first one is the type of the message, whether a <strong>
<span style="color:#eea2a2">Error</span></strong>, <strong><span style="color:#71f96c">Success</span></strong> or
<span style="color:#f0f17f"><strong>Notice</strong></span>. These influence which style the control uses. Also, you can change it at any time and the style will be updated accordingly; this is nice because you can use only one FlashMessage's instance to show
 different types of messages.</em></p>
<p><em>&nbsp; &nbsp; &nbsp;The second one it's the message itself. As soon as this property is changed, the FlashMessage fades in automatically, so you don't have to explicity call any method. Once the FlashMessage is shown you can click on the close button
 to fade it out or wait untill it fades out automatically. This behavior is configurable, as explained below.</em></p>
<p><em>&nbsp; &nbsp; &nbsp;Others properties affect the FlashMessage behavior, specifically how it fades out, here they are:</em></p>
<p><em>&nbsp; &nbsp; &nbsp;FadesOutAutomatically: besides clicking on the close button, you can configure whether it fades out automatically; the default is true.</em></p>
<p><em>&nbsp; &nbsp; &nbsp;FadeOutTime: it specifies how much time the FlashMessage waits to fades out after it fades in; this property only takes effect if FadesOutAutomatically is set to true; the default is 10 seconds.</em></p>
<h1>A Glimpse</h1>
<p><em>&nbsp; &nbsp; &nbsp;<img src="56088-sem%20t%c3%adtulo.png" alt="" width="520" height="181"></em></p>
<h1>The Sample</h1>
<p><em>&nbsp; &nbsp; &nbsp;The sample has two projects. One is a Control Library which contains the FlashMessage control, and the another one is a WPF Application that shows how to use it.</em></p>
<h1><span style="font-size:20px">The Code</span></h1>
<p><em>&nbsp; &nbsp; &nbsp;The code is divided in two parts, the implementation of the FlashMessage's behavior and its style.</em></p>
<p><em>&nbsp; &nbsp; &nbsp;If you open the Generic.xaml file, you'll notice VS designer will throw an error: &quot;Error<span>
</span>1<span> </span>Object 'null' cannot be used as an accessor parameter for a PropertyPath. An accessor parameter must be DependencyProperty, PropertyInfo, or PropertyDescriptor&quot;. This only happens on design mode and seems like a bug, as reported
<a title="connect.microsoft" href="http://connect.microsoft.com/VisualStudio/feedback/details/552131/designer-fails-when-data-binding-to-a-grandchild-property-through-an-explicit-interface-on-the-immediate-child" target="_blank">
here</a>.&nbsp;</em></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">xaml</span>
<pre class="hidden">using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FlashMessage
{
    public class FlashMessage : Control
    {
        private DispatcherTimer fadeOutTimer;

        static FlashMessage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlashMessage), new FrameworkPropertyMetadata(typeof(FlashMessage)));
        }

        public FlashMessage()
        {
            this.fadeOutTimer = new DispatcherTimer();
            this.fadeOutTimer.Interval = this.FadeOutTime.TimeSpan;
            this.fadeOutTimer.Tick &#43;= this.FadeOutTimer_Tick;
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommandHandler));
        }

        #region ContentDataTemplate
        public static DependencyProperty ContentDataTemplateProperty =
            DependencyProperty.Register(&quot;ContentDataTemplate&quot;, typeof(DataTemplate), typeof(FlashMessage));

        public DataTemplate ContentDataTemplate
        {
            get { return (DataTemplate)GetValue(ContentDataTemplateProperty); }
            set { SetValue(ContentDataTemplateProperty, value); }
        }
        #endregion

        #region FadesOutAutomatically
        public static DependencyProperty FadesOutAutomaticallyProperty =
            DependencyProperty.Register(&quot;FadesOutAutomatically&quot;, typeof(bool), typeof(FlashMessage), new PropertyMetadata(true));

        public bool FadesOutAutomatically
        {
            get { return (bool)GetValue(FadesOutAutomaticallyProperty); }
            set { SetValue(FadesOutAutomaticallyProperty, value); }
        }
        #endregion

        #region FadeOutTime
        public static DependencyProperty FadeOutTimeProperty =
            DependencyProperty.Register(&quot;FadeOutTime&quot;, typeof(Duration), typeof(FlashMessage),
            new PropertyMetadata(new Duration(TimeSpan.FromSeconds(10)), new PropertyChangedCallback(OnFadeOutTimeChanged)));

        public Duration FadeOutTime
        {
            get { return (Duration)GetValue(FadeOutTimeProperty); }
            set { SetValue(FadeOutTimeProperty, value); }
        }

        private static void OnFadeOutTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((FlashMessage)o).OnFadeOutTimeChanged();
        }

        private void OnFadeOutTimeChanged()
        {
            this.fadeOutTimer.Interval = this.FadeOutTime.TimeSpan;
            this.StopTimerIfRunning();
        }
        #endregion

        #region Message
        public static DependencyProperty MessageProperty =
            DependencyProperty.Register(&quot;Message&quot;, typeof(string), typeof(FlashMessage), new PropertyMetadata(OnMessageChanged));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        private static void OnMessageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((FlashMessage)o).OnMessageChanged();
        }

        private void OnMessageChanged()
        {
            this.IsFlashMessageVisible = !string.IsNullOrEmpty(this.Message);
        }
        #endregion

        #region MessageType
        public static DependencyProperty MessageTypeProperty =
            DependencyProperty.Register(&quot;MessageType&quot;, typeof(MessageType), typeof(FlashMessage));

        public MessageType MessageType
        {
            get { return (MessageType)GetValue(MessageTypeProperty); }
            set { SetValue(MessageTypeProperty, value); }
        }
        #endregion

        #region Reset
        internal static DependencyProperty ResetProperty =
          DependencyProperty.Register(&quot;Reset&quot;, typeof(bool), typeof(FlashMessage), new PropertyMetadata(OnResetChanged));

        internal bool Reset
        {
            get { return (bool)GetValue(ResetProperty); }
            set { SetValue(ResetProperty, value); }
        }

        private static void OnResetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((FlashMessage)o).OnResetChanged();
        }

        private void OnResetChanged()
        {
            if (this.Reset)
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
                this.Message = null;
            }
        }
        #endregion

        #region IsFlashMessageVisible
        internal static DependencyProperty IsFlashMessageVisibleProperty =
          DependencyProperty.Register(&quot;IsFlashMessageVisible&quot;, typeof(bool), typeof(FlashMessage), new PropertyMetadata(OnIsFlashMessageVisibleChanged));

        internal bool IsFlashMessageVisible
        {
            get { return (bool)GetValue(IsFlashMessageVisibleProperty); }
            set { SetValue(IsFlashMessageVisibleProperty, value); }
        }

        private static void OnIsFlashMessageVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((FlashMessage)o).OnIsFlashMessageVisibleChanged();
        }

        private void OnIsFlashMessageVisibleChanged()
        {
            if (this.IsFlashMessageVisible)
            {
                this.Visibility = System.Windows.Visibility.Visible;
                this.StartTimerIfAutomatically();
            }
            else
                this.StartFadeOutAnimation();
        }
        #endregion

        public void Hide()
        {
            this.StopTimerIfRunning();
            this.IsFlashMessageVisible = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.ContentDataTemplate == null)
                this.ContentDataTemplate = (DataTemplate)this.FindResource(&quot;flashMessageTemplate&quot;);
        }

        private void FadeOutTimer_Tick(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Hide();
        }

        private void StopTimerIfRunning()
        {
            if (this.fadeOutTimer.IsEnabled)
                this.fadeOutTimer.Stop();
        }

        private void StartTimerIfAutomatically()
        {
            this.StopTimerIfRunning();
            if (this.FadesOutAutomatically)
                this.fadeOutTimer.Start();
        }

        private void StartFadeOutAnimation()
        {
            var storyBoard = (Storyboard)this.FindResource(&quot;fadeOutAnimation&quot;);
            if (storyBoard != null)
                storyBoard.Begin(this);
        }
    }
}
</pre>
<pre class="hidden">&lt;ResourceDictionary xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
                    xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
                    xmlns:local=&quot;clr-namespace:FlashMessage&quot;&gt;

    &lt;LinearGradientBrush x:Key=&quot;flashMessageErrorBackground&quot; StartPoint=&quot;0.5,0&quot; EndPoint=&quot;0.5,1&quot;&gt;
        &lt;GradientStop Color=&quot;#FFF2C6C6&quot;/&gt;
        &lt;GradientStop Color=&quot;#FFEEA2A2&quot; Offset=&quot;1&quot;/&gt;
        &lt;GradientStop Color=&quot;#FFF0B6B6&quot; Offset=&quot;0.487&quot;/&gt;
        &lt;GradientStop Color=&quot;#FFF1BDBD&quot; Offset=&quot;0.228&quot;/&gt;
        &lt;GradientStop Color=&quot;#FFEFABAB&quot; Offset=&quot;0.797&quot;/&gt;
    &lt;/LinearGradientBrush&gt;

    &lt;LinearGradientBrush x:Key=&quot;flashMessageSuccessBackground&quot; StartPoint=&quot;0.5,0&quot; EndPoint=&quot;0.5,1&quot;&gt;
        &lt;GradientStop Color=&quot;#FF71F96C&quot;/&gt;
        &lt;GradientStop Color=&quot;#FF96F696&quot; Offset=&quot;1&quot;/&gt;
        &lt;GradientStop Color=&quot;#FF7BFF7C&quot; Offset=&quot;0.487&quot;/&gt;
        &lt;GradientStop Color=&quot;#FF6AFF6A&quot; Offset=&quot;0.228&quot;/&gt;
        &lt;GradientStop Color=&quot;#FF89FF89&quot; Offset=&quot;0.797&quot;/&gt;
    &lt;/LinearGradientBrush&gt;

    &lt;LinearGradientBrush x:Key=&quot;flashMessageNoticeBackground&quot; StartPoint=&quot;0.5,0&quot; EndPoint=&quot;0.5,1&quot;&gt;
        &lt;GradientStop Color=&quot;#FFFDFEAD&quot;/&gt;
        &lt;GradientStop Color=&quot;#FFF0F17F&quot; Offset=&quot;1&quot;/&gt;
        &lt;GradientStop Color=&quot;#FFF6F797&quot; Offset=&quot;0.487&quot;/&gt;
        &lt;GradientStop Color=&quot;#FFFAFBA3&quot; Offset=&quot;0.228&quot;/&gt;
        &lt;GradientStop Color=&quot;#FFF3F48C&quot; Offset=&quot;0.797&quot;/&gt;
    &lt;/LinearGradientBrush&gt;

    &lt;Style TargetType=&quot;Image&quot; x:Key=&quot;flashMessageImageButton&quot;&gt;
        &lt;Setter Property=&quot;HorizontalAlignment&quot; Value=&quot;Center&quot;/&gt;
        &lt;Setter Property=&quot;VerticalAlignment&quot; Value=&quot;Center&quot;/&gt;
        &lt;Setter Property=&quot;Source&quot; Value=&quot;pack://application:,,,/FlashMessage;component/Images/close_8.png&quot;/&gt;
        &lt;Setter Property=&quot;Stretch&quot; Value=&quot;None&quot;/&gt;
    &lt;/Style&gt;
    &lt;Style TargetType=&quot;Button&quot; x:Key=&quot;flashMessageButton&quot;&gt;
        &lt;Setter Property=&quot;Margin&quot; Value=&quot;0,0,5,0&quot;/&gt;
        &lt;Setter Property=&quot;BorderThickness&quot; Value=&quot;1&quot;/&gt;
        &lt;Setter Property=&quot;Width&quot; Value=&quot;22&quot;/&gt;
        &lt;Setter Property=&quot;Height&quot; Value=&quot;22&quot;/&gt;
    &lt;/Style&gt;

    &lt;!--TODO: ColorAnimation.To={TemplateBinding BorderBrush}; currently this won't work.--&gt;
    &lt;Style TargetType=&quot;Button&quot; BasedOn=&quot;{StaticResource flashMessageButton}&quot; x:Key=&quot;flashMessageErrorButton&quot;&gt;
        &lt;Setter Property=&quot;BorderBrush&quot; Value=&quot;#E89D9D&quot;/&gt;
        &lt;Setter Property=&quot;Template&quot;&gt;
            &lt;Setter.Value&gt;
                &lt;ControlTemplate TargetType=&quot;Button&quot;&gt;
                    &lt;Grid&gt;
                        &lt;Ellipse x:Name=&quot;imageButtonBorder&quot; Fill=&quot;Transparent&quot; StrokeThickness=&quot;{TemplateBinding BorderThickness}&quot; Stroke=&quot;{TemplateBinding BorderBrush}&quot;/&gt;
                        &lt;Image Style=&quot;{StaticResource flashMessageImageButton}&quot; /&gt;

                        &lt;VisualStateManager.VisualStateGroups&gt;
                            &lt;VisualStateGroup x:Name=&quot;Common&quot;&gt;
                                &lt;VisualState x:Name=&quot;MouseOver&quot;&gt;
                                    &lt;Storyboard&gt;
                                        &lt;ColorAnimation Storyboard.TargetName=&quot;imageButtonBorder&quot; Storyboard.TargetProperty=&quot;Fill.Color&quot; To=&quot;#E89D9D&quot; Duration=&quot;0:00:0.3&quot; /&gt;
                                    &lt;/Storyboard&gt;
                                &lt;/VisualState&gt;
                                &lt;VisualState x:Name=&quot;Normal&quot;&gt;
                                    &lt;Storyboard&gt;
                                        &lt;ColorAnimation Storyboard.TargetName=&quot;imageButtonBorder&quot; Storyboard.TargetProperty=&quot;Fill.Color&quot; To=&quot;Transparent&quot; Duration=&quot;0:00:0.3&quot; /&gt;
                                    &lt;/Storyboard&gt;
                                &lt;/VisualState&gt;
                            &lt;/VisualStateGroup&gt;
                        &lt;/VisualStateManager.VisualStateGroups&gt;
                    &lt;/Grid&gt;
                &lt;/ControlTemplate&gt;
            &lt;/Setter.Value&gt;
        &lt;/Setter&gt;
    &lt;/Style&gt;
    &lt;Style TargetType=&quot;Button&quot; BasedOn=&quot;{StaticResource flashMessageButton}&quot; x:Key=&quot;flashMessageSuccessButton&quot;&gt;
        &lt;Setter Property=&quot;BorderBrush&quot; Value=&quot;#52ED52&quot;/&gt;
        &lt;Setter Property=&quot;Template&quot;&gt;
            &lt;Setter.Value&gt;
                &lt;ControlTemplate TargetType=&quot;Button&quot;&gt;
                    &lt;Grid&gt;
                        &lt;Ellipse x:Name=&quot;imageButtonBorder&quot; Fill=&quot;Transparent&quot; StrokeThickness=&quot;{TemplateBinding BorderThickness}&quot; Stroke=&quot;{TemplateBinding BorderBrush}&quot;/&gt;
                        &lt;Image Style=&quot;{StaticResource flashMessageImageButton}&quot; /&gt;

                        &lt;VisualStateManager.VisualStateGroups&gt;
                            &lt;VisualStateGroup x:Name=&quot;Common&quot;&gt;
                                &lt;VisualState x:Name=&quot;MouseOver&quot;&gt;
                                    &lt;Storyboard&gt;
                                        &lt;ColorAnimation Storyboard.TargetName=&quot;imageButtonBorder&quot; Storyboard.TargetProperty=&quot;Fill.Color&quot; To=&quot;#52ED52&quot; Duration=&quot;0:00:0.3&quot; /&gt;
                                    &lt;/Storyboard&gt;
                                &lt;/VisualState&gt;
                                &lt;VisualState x:Name=&quot;Normal&quot;&gt;
                                    &lt;Storyboard&gt;
                                        &lt;ColorAnimation Storyboard.TargetName=&quot;imageButtonBorder&quot; Storyboard.TargetProperty=&quot;Fill.Color&quot; To=&quot;Transparent&quot; Duration=&quot;0:00:0.3&quot; /&gt;
                                    &lt;/Storyboard&gt;
                                &lt;/VisualState&gt;
                            &lt;/VisualStateGroup&gt;
                        &lt;/VisualStateManager.VisualStateGroups&gt;
                    &lt;/Grid&gt;
                &lt;/ControlTemplate&gt;
            &lt;/Setter.Value&gt;
        &lt;/Setter&gt;
    &lt;/Style&gt;
    &lt;Style TargetType=&quot;Button&quot; BasedOn=&quot;{StaticResource flashMessageButton}&quot; x:Key=&quot;flashMessageNoticeButton&quot;&gt;
        &lt;Setter Property=&quot;BorderBrush&quot; Value=&quot;#FFEEEF65&quot;/&gt;
        &lt;Setter Property=&quot;Template&quot;&gt;
            &lt;Setter.Value&gt;
                &lt;ControlTemplate TargetType=&quot;Button&quot;&gt;
                    &lt;Grid&gt;
                        &lt;Ellipse x:Name=&quot;imageButtonBorder&quot; Fill=&quot;Transparent&quot; StrokeThickness=&quot;{TemplateBinding BorderThickness}&quot; Stroke=&quot;{TemplateBinding BorderBrush}&quot;/&gt;
                        &lt;Image Style=&quot;{StaticResource flashMessageImageButton}&quot; /&gt;

                        &lt;VisualStateManager.VisualStateGroups&gt;
                            &lt;VisualStateGroup x:Name=&quot;Common&quot;&gt;
                                &lt;VisualState x:Name=&quot;MouseOver&quot;&gt;
                                    &lt;Storyboard&gt;
                                        &lt;ColorAnimation Storyboard.TargetName=&quot;imageButtonBorder&quot; Storyboard.TargetProperty=&quot;Fill.Color&quot; To=&quot;#FFEEEF65&quot; Duration=&quot;0:00:0.3&quot; /&gt;
                                    &lt;/Storyboard&gt;
                                &lt;/VisualState&gt;
                                &lt;VisualState x:Name=&quot;Normal&quot;&gt;
                                    &lt;Storyboard&gt;
                                        &lt;ColorAnimation Storyboard.TargetName=&quot;imageButtonBorder&quot; Storyboard.TargetProperty=&quot;Fill.Color&quot; To=&quot;Transparent&quot; Duration=&quot;0:00:0.3&quot; /&gt;
                                    &lt;/Storyboard&gt;
                                &lt;/VisualState&gt;
                            &lt;/VisualStateGroup&gt;
                        &lt;/VisualStateManager.VisualStateGroups&gt;
                    &lt;/Grid&gt;
                &lt;/ControlTemplate&gt;
            &lt;/Setter.Value&gt;
        &lt;/Setter&gt;
    &lt;/Style&gt;

    &lt;Style TargetType=&quot;{x:Type local:FlashMessage}&quot;&gt;
        &lt;Setter Property=&quot;Visibility&quot; Value=&quot;Collapsed&quot;/&gt;
        &lt;Setter Property=&quot;MinHeight&quot; Value=&quot;40&quot;/&gt;
        &lt;Setter Property=&quot;IsTabStop&quot; Value=&quot;False&quot;/&gt;
        &lt;Setter Property=&quot;Focusable&quot; Value=&quot;False&quot;/&gt;
        &lt;Setter Property=&quot;BorderThickness&quot; Value=&quot;1&quot;/&gt;
        &lt;Setter Property=&quot;Margin&quot; Value=&quot;0,0,0,10&quot;/&gt;
        &lt;Setter Property=&quot;Template&quot;&gt;
            &lt;Setter.Value&gt;
                &lt;ControlTemplate TargetType=&quot;{x:Type local:FlashMessage}&quot;&gt;
                    &lt;Border x:Name=&quot;PART_border&quot; CornerRadius=&quot;2&quot; BorderThickness=&quot;{TemplateBinding BorderThickness}&quot; Margin=&quot;{TemplateBinding Margin}&quot;&gt;
                        &lt;Grid&gt;
                            &lt;Grid.ColumnDefinitions&gt;
                                &lt;ColumnDefinition Width=&quot;*&quot;/&gt;
                                &lt;ColumnDefinition Width=&quot;50&quot;/&gt;
                            &lt;/Grid.ColumnDefinitions&gt;

                            &lt;ContentControl Grid.Column=&quot;0&quot; ContentTemplate=&quot;{TemplateBinding ContentDataTemplate}&quot;
                                            Content=&quot;{TemplateBinding Message}&quot;/&gt;
                            &lt;Button Grid.Column=&quot;1&quot; x:Name=&quot;PART_button&quot; Cursor=&quot;Hand&quot; ToolTip=&quot;Click to hide&quot; Command=&quot;Close&quot;/&gt;
                        &lt;/Grid&gt;
                    &lt;/Border&gt;
                    &lt;ControlTemplate.Triggers&gt;
                        &lt;Trigger Property=&quot;MessageType&quot; Value=&quot;Error&quot;&gt;
                            &lt;Setter Property=&quot;Background&quot; TargetName=&quot;PART_border&quot; Value=&quot;{StaticResource flashMessageErrorBackground}&quot;/&gt;
                            &lt;Setter Property=&quot;Style&quot; TargetName=&quot;PART_button&quot; Value=&quot;{StaticResource flashMessageErrorButton}&quot;/&gt;
                        &lt;/Trigger&gt;
                        &lt;Trigger Property=&quot;MessageType&quot; Value=&quot;Success&quot;&gt;
                            &lt;Setter Property=&quot;Background&quot; TargetName=&quot;PART_border&quot; Value=&quot;{StaticResource flashMessageSuccessBackground}&quot;/&gt;
                            &lt;Setter Property=&quot;Style&quot; TargetName=&quot;PART_button&quot; Value=&quot;{StaticResource flashMessageSuccessButton}&quot;/&gt;
                        &lt;/Trigger&gt;
                        &lt;Trigger Property=&quot;MessageType&quot; Value=&quot;Notice&quot;&gt;
                            &lt;Setter Property=&quot;Background&quot; TargetName=&quot;PART_border&quot; Value=&quot;{StaticResource flashMessageNoticeBackground}&quot;/&gt;
                            &lt;Setter Property=&quot;Style&quot; TargetName=&quot;PART_button&quot; Value=&quot;{StaticResource flashMessageNoticeButton}&quot;/&gt;
                        &lt;/Trigger&gt;
                    &lt;/ControlTemplate.Triggers&gt;
                &lt;/ControlTemplate&gt;
            &lt;/Setter.Value&gt;
        &lt;/Setter&gt;
        &lt;Style.Resources&gt;
            &lt;DataTemplate x:Key=&quot;flashMessageTemplate&quot; DataType=&quot;string&quot;&gt;
                &lt;TextBlock Text=&quot;{Binding}&quot; FontSize=&quot;11&quot; FontStyle=&quot;Italic&quot; Padding=&quot;2&quot; Margin=&quot;5,0,0,0&quot; HorizontalAlignment=&quot;Left&quot; VerticalAlignment=&quot;Center&quot; /&gt;
            &lt;/DataTemplate&gt;
            &lt;Storyboard x:Key=&quot;fadeOutAnimation&quot;&gt;
                &lt;DoubleAnimation FillBehavior=&quot;Stop&quot; Storyboard.TargetProperty=&quot;Opacity&quot; From=&quot;1.0&quot; To=&quot;0.0&quot; Duration=&quot;0:0:1.3&quot; /&gt;
                &lt;BooleanAnimationUsingKeyFrames Storyboard.TargetProperty=&quot;(local:FlashMessage.Reset)&quot;&gt;
                    &lt;DiscreteBooleanKeyFrame KeyTime=&quot;0:0:0&quot; Value=&quot;False&quot;/&gt;
                    &lt;DiscreteBooleanKeyFrame KeyTime=&quot;0:0:1.3&quot; Value=&quot;True&quot;/&gt;
                &lt;/BooleanAnimationUsingKeyFrames&gt;
            &lt;/Storyboard&gt;
        &lt;/Style.Resources&gt;
        &lt;Style.Triggers&gt;
            &lt;Trigger Property=&quot;Visibility&quot; Value=&quot;Visible&quot;&gt;
                &lt;Trigger.EnterActions&gt;
                    &lt;BeginStoryboard&gt;
                        &lt;Storyboard&gt;
                            &lt;DoubleAnimation Storyboard.TargetProperty=&quot;Opacity&quot; From=&quot;0.0&quot; To=&quot;1.0&quot; Duration=&quot;0:0:1.3&quot; /&gt;
                        &lt;/Storyboard&gt;
                    &lt;/BeginStoryboard&gt;
                &lt;/Trigger.EnterActions&gt;
            &lt;/Trigger&gt;
        &lt;/Style.Triggers&gt;
    &lt;/Style&gt;

&lt;/ResourceDictionary&gt;</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">using</span>&nbsp;System;&nbsp;
<span class="cs__keyword">using</span>&nbsp;System.Windows;&nbsp;
<span class="cs__keyword">using</span>&nbsp;System.Windows.Controls;&nbsp;
<span class="cs__keyword">using</span>&nbsp;System.Windows.Input;&nbsp;
<span class="cs__keyword">using</span>&nbsp;System.Windows.Media.Animation;&nbsp;
<span class="cs__keyword">using</span>&nbsp;System.Windows.Threading;&nbsp;
&nbsp;
<span class="cs__keyword">namespace</span>&nbsp;FlashMessage&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;FlashMessage&nbsp;:&nbsp;Control&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;DispatcherTimer&nbsp;fadeOutTimer;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">static</span>&nbsp;FlashMessage()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DefaultStyleKeyProperty.OverrideMetadata(<span class="cs__keyword">typeof</span>(FlashMessage),&nbsp;<span class="cs__keyword">new</span>&nbsp;FrameworkPropertyMetadata(<span class="cs__keyword">typeof</span>(FlashMessage)));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;FlashMessage()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.fadeOutTimer&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DispatcherTimer();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.fadeOutTimer.Interval&nbsp;=&nbsp;<span class="cs__keyword">this</span>.FadeOutTime.TimeSpan;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.fadeOutTimer.Tick&nbsp;&#43;=&nbsp;<span class="cs__keyword">this</span>.FadeOutTimer_Tick;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.CommandBindings.Add(<span class="cs__keyword">new</span>&nbsp;CommandBinding(ApplicationCommands.Close,&nbsp;CloseCommandHandler));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region&nbsp;ContentDataTemplate</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;DependencyProperty&nbsp;ContentDataTemplateProperty&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.Register(<span class="cs__string">&quot;ContentDataTemplate&quot;</span>,&nbsp;<span class="cs__keyword">typeof</span>(DataTemplate),&nbsp;<span class="cs__keyword">typeof</span>(FlashMessage));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;DataTemplate&nbsp;ContentDataTemplate&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;(DataTemplate)GetValue(ContentDataTemplateProperty);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;SetValue(ContentDataTemplateProperty,&nbsp;<span class="cs__keyword">value</span>);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span><span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region&nbsp;FadesOutAutomatically</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;DependencyProperty&nbsp;FadesOutAutomaticallyProperty&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.Register(<span class="cs__string">&quot;FadesOutAutomatically&quot;</span>,&nbsp;<span class="cs__keyword">typeof</span>(<span class="cs__keyword">bool</span>),&nbsp;<span class="cs__keyword">typeof</span>(FlashMessage),&nbsp;<span class="cs__keyword">new</span>&nbsp;PropertyMetadata(<span class="cs__keyword">true</span>));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;FadesOutAutomatically&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;(<span class="cs__keyword">bool</span>)GetValue(FadesOutAutomaticallyProperty);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;SetValue(FadesOutAutomaticallyProperty,&nbsp;<span class="cs__keyword">value</span>);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span><span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region&nbsp;FadeOutTime</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;DependencyProperty&nbsp;FadeOutTimeProperty&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.Register(<span class="cs__string">&quot;FadeOutTime&quot;</span>,&nbsp;<span class="cs__keyword">typeof</span>(Duration),&nbsp;<span class="cs__keyword">typeof</span>(FlashMessage),&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">new</span>&nbsp;PropertyMetadata(<span class="cs__keyword">new</span>&nbsp;Duration(TimeSpan.FromSeconds(<span class="cs__number">10</span>)),&nbsp;<span class="cs__keyword">new</span>&nbsp;PropertyChangedCallback(OnFadeOutTimeChanged)));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;Duration&nbsp;FadeOutTime&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;(Duration)GetValue(FadeOutTimeProperty);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;SetValue(FadeOutTimeProperty,&nbsp;<span class="cs__keyword">value</span>);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnFadeOutTimeChanged(DependencyObject&nbsp;o,&nbsp;DependencyPropertyChangedEventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;((FlashMessage)o).OnFadeOutTimeChanged();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnFadeOutTimeChanged()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.fadeOutTimer.Interval&nbsp;=&nbsp;<span class="cs__keyword">this</span>.FadeOutTime.TimeSpan;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StopTimerIfRunning();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span><span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region&nbsp;Message</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;DependencyProperty&nbsp;MessageProperty&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.Register(<span class="cs__string">&quot;Message&quot;</span>,&nbsp;<span class="cs__keyword">typeof</span>(<span class="cs__keyword">string</span>),&nbsp;<span class="cs__keyword">typeof</span>(FlashMessage),&nbsp;<span class="cs__keyword">new</span>&nbsp;PropertyMetadata(OnMessageChanged));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;Message&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;(<span class="cs__keyword">string</span>)GetValue(MessageProperty);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;SetValue(MessageProperty,&nbsp;<span class="cs__keyword">value</span>);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnMessageChanged(DependencyObject&nbsp;o,&nbsp;DependencyPropertyChangedEventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;((FlashMessage)o).OnMessageChanged();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnMessageChanged()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.IsFlashMessageVisible&nbsp;=&nbsp;!<span class="cs__keyword">string</span>.IsNullOrEmpty(<span class="cs__keyword">this</span>.Message);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span><span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region&nbsp;MessageType</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;DependencyProperty&nbsp;MessageTypeProperty&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.Register(<span class="cs__string">&quot;MessageType&quot;</span>,&nbsp;<span class="cs__keyword">typeof</span>(MessageType),&nbsp;<span class="cs__keyword">typeof</span>(FlashMessage));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;MessageType&nbsp;MessageType&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;(MessageType)GetValue(MessageTypeProperty);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;SetValue(MessageTypeProperty,&nbsp;<span class="cs__keyword">value</span>);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span><span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region&nbsp;Reset</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">internal</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;DependencyProperty&nbsp;ResetProperty&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.Register(<span class="cs__string">&quot;Reset&quot;</span>,&nbsp;<span class="cs__keyword">typeof</span>(<span class="cs__keyword">bool</span>),&nbsp;<span class="cs__keyword">typeof</span>(FlashMessage),&nbsp;<span class="cs__keyword">new</span>&nbsp;PropertyMetadata(OnResetChanged));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">internal</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;Reset&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;(<span class="cs__keyword">bool</span>)GetValue(ResetProperty);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;SetValue(ResetProperty,&nbsp;<span class="cs__keyword">value</span>);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnResetChanged(DependencyObject&nbsp;o,&nbsp;DependencyPropertyChangedEventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;((FlashMessage)o).OnResetChanged();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnResetChanged()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(<span class="cs__keyword">this</span>.Reset)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.Visibility&nbsp;=&nbsp;System.Windows.Visibility.Collapsed;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.Message&nbsp;=&nbsp;<span class="cs__keyword">null</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span><span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#region&nbsp;IsFlashMessageVisible</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">internal</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;DependencyProperty&nbsp;IsFlashMessageVisibleProperty&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.Register(<span class="cs__string">&quot;IsFlashMessageVisible&quot;</span>,&nbsp;<span class="cs__keyword">typeof</span>(<span class="cs__keyword">bool</span>),&nbsp;<span class="cs__keyword">typeof</span>(FlashMessage),&nbsp;<span class="cs__keyword">new</span>&nbsp;PropertyMetadata(OnIsFlashMessageVisibleChanged));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">internal</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;IsFlashMessageVisible&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;(<span class="cs__keyword">bool</span>)GetValue(IsFlashMessageVisibleProperty);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;SetValue(IsFlashMessageVisibleProperty,&nbsp;<span class="cs__keyword">value</span>);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnIsFlashMessageVisibleChanged(DependencyObject&nbsp;o,&nbsp;DependencyPropertyChangedEventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;((FlashMessage)o).OnIsFlashMessageVisibleChanged();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnIsFlashMessageVisibleChanged()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(<span class="cs__keyword">this</span>.IsFlashMessageVisible)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.Visibility&nbsp;=&nbsp;System.Windows.Visibility.Visible;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StartTimerIfAutomatically();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StartFadeOutAnimation();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Hide()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StopTimerIfRunning();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.IsFlashMessageVisible&nbsp;=&nbsp;<span class="cs__keyword">false</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">override</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnApplyTemplate()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">base</span>.OnApplyTemplate();&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(<span class="cs__keyword">this</span>.ContentDataTemplate&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.ContentDataTemplate&nbsp;=&nbsp;(DataTemplate)<span class="cs__keyword">this</span>.FindResource(<span class="cs__string">&quot;flashMessageTemplate&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;FadeOutTimer_Tick(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.Hide();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;CloseCommandHandler(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;ExecutedRoutedEventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.Hide();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;StopTimerIfRunning()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(<span class="cs__keyword">this</span>.fadeOutTimer.IsEnabled)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.fadeOutTimer.Stop();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;StartTimerIfAutomatically()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StopTimerIfRunning();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(<span class="cs__keyword">this</span>.FadesOutAutomatically)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.fadeOutTimer.Start();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;StartFadeOutAnimation()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;storyBoard&nbsp;=&nbsp;(Storyboard)<span class="cs__keyword">this</span>.FindResource(<span class="cs__string">&quot;fadeOutAnimation&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(storyBoard&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;storyBoard.Begin(<span class="cs__keyword">this</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<h1>More Information</h1>
<p><em>&nbsp; &nbsp; &nbsp;This project is also on <a href="https://github.com/JobaDiniz/FlashMessage" target="_blank">
GitHub</a>; feel free to fork it and help to improve the FlashMessage control.</em></p>