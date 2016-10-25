namespace JMobile.Droid.Views
{
    using Android.Content;
    using Android.Content.Res;
    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.Graphics.Drawables.Shapes;
    using Android.Util;
    using Android.Views;
    using Android.Views.Animations;
    using Android.Widget;

    public class BadgeView : TextView
    {
        public const int POSITION_TOP_LEFT = 1;
        public const int POSITION_TOP_RIGHT = 2;
        public const int POSITION_BOTTOM_LEFT = 3;
        public const int POSITION_BOTTOM_RIGHT = 4;
        public const int POSITION_CENTER = 5;

        private const int DEFAULT_MARGIN_DIP = 0;
        private const int DEFAULT_LR_PADDING_DIP = 5;
        private const int DEFAULT_CORNER_RADIUS_DIP = 4;
        private const int DEFAULT_POSITION = POSITION_TOP_RIGHT;
        private static Color DEFAULT_BADGE_COLOR = Color.Red;
        private static Color DEFAULT_TEXT_COLOR = Color.White;

        private static Animation fadeIn;
        private static Animation fadeOut;

        private Context context;
        private View target;

        private int badgePosition;
        private int badgeMarginH;
        private int badgeMarginV;
        private Color badgeColor;

        private bool isShown;

        private ShapeDrawable badgeBg;

        private int targetTabIndex;

        public BadgeView(Context context)
            : this(context, (Android.Util.IAttributeSet)null, Android.Resource.Attribute.TextViewStyle)
        { }

        public BadgeView(Context context, IAttributeSet attrs)
            : this(context, attrs, Android.Resource.Attribute.TextViewStyle)
        { }

        public BadgeView(Context context, View target)
            : this(context, null, Android.Resource.Attribute.TextViewStyle, target, 0)
        { }

        public BadgeView(Context context, TabWidget target, int index)
            : this(context, null, Android.Resource.Attribute.TextViewStyle, target, index)
        { }

        public BadgeView(Context context, IAttributeSet attrs, int defStyle)
            : this(context, attrs, defStyle, null, 0)
        { }

        public BadgeView(Context context, IAttributeSet attrs, int defStyle, View target, int tabIndex)
            : base(context, attrs, defStyle)
        {
            init(context, target, tabIndex);
        }

        private void init(Context context, View target, int tabIndex)
        {
            this.context = context;
            this.target = target;
            this.targetTabIndex = tabIndex;

            badgePosition = DEFAULT_POSITION;
            badgeMarginH = dipToPixels(DEFAULT_MARGIN_DIP);
            badgeMarginV = badgeMarginH;
            badgeColor = DEFAULT_BADGE_COLOR;

            SetTypeface(Typeface.DefaultBold, TypefaceStyle.Bold);
            int paddingPixels = dipToPixels(DEFAULT_LR_PADDING_DIP);
            SetPadding(paddingPixels, 0, paddingPixels, 0);
            SetTextColor(DEFAULT_TEXT_COLOR);

            fadeIn = new AlphaAnimation(0, 1)
            {
                Interpolator = new DecelerateInterpolator(),
                Duration = 200
            };

            fadeOut = new AlphaAnimation(1, 0)
            {
                Interpolator = new DecelerateInterpolator(),
                Duration = 200
            };

            isShown = false;

            if (this.target != null)
            {
                applyTo(this.target);
            }
            else
            {
                Show();
            }
        }

        private int dipToPixels(int dip)
        {
            Resources r = Resources;
            float px = TypedValue.ApplyDimension(ComplexUnitType.Dip, dip, r.DisplayMetrics);
            return (int)px;
        }

        private void applyTo(View target)
        {
            var lp = target.LayoutParameters;
            var parent = target.Parent;
            var container = new FrameLayout(context);

            if (target.GetType() == typeof(TabWidget))
            {
                target = ((TabWidget)target).GetChildTabViewAt(targetTabIndex);
                this.target = target;
                ((ViewGroup)target).AddView(container, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
                this.Visibility = ViewStates.Gone;
                container.AddView(this);
            }
            else
            {
                var group = (ViewGroup)parent;
                int index = group.IndexOfChild(target);

                group.RemoveView(target);
                group.AddView(container, index, lp);

                container.AddView(target);

                this.Visibility = ViewStates.Gone;
                container.AddView(this);

                group.Invalidate();
            }
        }

        public void Show()
        {
            Show(false, null);
        }

        public void Show(bool animate)
        {
            Show(animate, fadeIn);
        }

        public void Show(Animation anim)
        {
            Show(true, anim);
        }

        private void Show(bool animate, Animation anim)
        {
            if (Background == null)
            {
                if (badgeBg == null)
                {
                    badgeBg = getDefaultBackground();
                }

                SetBackgroundDrawable(badgeBg);
            }

            applyLayoutParams();

            if (animate)
            {
                this.StartAnimation(anim);
            }
            this.Visibility = ViewStates.Visible;
            isShown = true;
        }

        public void Hide()
        {
            Hide(false, null);
        }

        public void Hide(bool animate)
        {
            Hide(animate, fadeOut);
        }

        public void Hide(Animation anim)
        {
            Hide(true, anim);
        }

        private void Hide(bool animate, Animation anim)
        {
            this.Visibility = ViewStates.Gone;
            if (animate)
            {
                this.StartAnimation(anim);
            }
            isShown = false;
        }

        public void Toggle()
        {
            Toggle(false, null, null);
        }

        public void Toggle(bool animate)
        {
            Toggle(animate, fadeIn, fadeOut);
        }

        public void Toggle(Animation animIn, Animation animOut)
        {
            Toggle(true, animIn, animOut);
        }

        private void Toggle(bool animate, Animation animIn, Animation animOut)
        {
            if (isShown)
            {
                Hide(animate && (animOut != null), animOut);
            }
            else
            {
                Show(animate && (animIn != null), animIn);
            }
        }

        private ShapeDrawable getDefaultBackground()
        {
            int r = dipToPixels(DEFAULT_CORNER_RADIUS_DIP);
            float[] outerR = new float[] { r, r, r, r, r, r, r, r };

            var rr = new RoundRectShape(outerR, null, null);
            ShapeDrawable drawable = new ShapeDrawable(rr);
            drawable.Paint.Color = badgeColor;

            return drawable;
        }

        private void applyLayoutParams()
        {
            FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, FrameLayout.LayoutParams.WrapContent);

            switch (badgePosition)
            {
                case POSITION_TOP_LEFT:
                    lp.Gravity = GravityFlags.Left | GravityFlags.Top;
                    lp.SetMargins(badgeMarginH, badgeMarginV, 0, 0);
                    break;
                case POSITION_TOP_RIGHT:
                    lp.Gravity = GravityFlags.Right | GravityFlags.Top;
                    lp.SetMargins(0, badgeMarginV, badgeMarginH, 0);
                    break;
                case POSITION_BOTTOM_LEFT:
                    lp.Gravity = GravityFlags.Left | GravityFlags.Bottom;
                    lp.SetMargins(badgeMarginH, 0, 0, badgeMarginV);
                    break;
                case POSITION_BOTTOM_RIGHT:
                    lp.Gravity = GravityFlags.Right | GravityFlags.Bottom;
                    lp.SetMargins(0, 0, badgeMarginH, badgeMarginV);
                    break;
                case POSITION_CENTER:
                    lp.Gravity = GravityFlags.Center;
                    lp.SetMargins(0, 0, 0, 0);
                    break;
                default:
                    break;
            }

            LayoutParameters = lp;
        }

        public View GetTarget()
        {
            return target;
        }

        public bool IsShown()
        {
            return isShown;
        }

        public int GetBadgePosition()
        {
            return badgePosition;
        }

        public void SetBadgePosition(int layoutPosition)
        {
            this.badgePosition = layoutPosition;
        }

        public int GetHorizontalBadgeMargin()
        {
            return badgeMarginH;
        }

        public int GetVerticalBadgeMargin()
        {
            return badgeMarginV;
        }

        public void SetBadgeMargin(int badgeMargin)
        {
            this.badgeMarginH = badgeMargin;
            this.badgeMarginV = badgeMargin;
        }

        public void SetBadgeMargin(int horizontal, int vertical)
        {
            this.badgeMarginH = horizontal;
            this.badgeMarginV = vertical;
        }

        public int GetBadgeBackgroundColor()
        {
            return badgeColor;
        }

        public void SetBadgeBackgroundColor(Color badgeColor)
        {
            this.badgeColor = badgeColor;
            badgeBg = getDefaultBackground();
        }
    }
}