# Xamarin.BadgeView

Simple implementation in C# / Xamarin.Android of Badge Count to add at runtime without change the layout.

### Usage

```sh
var btnSample = FindViewById<ImageButton>(Resource.Id.btn_sample);
var badgeTarget = new BadgeView(this, btnSample);
badgeTarget.Text = "10";
badgeTarget.Show();
```

### Todos

 - Write a Demo
 - Add Code Comments
 - Improve functionality
 - Improve documentation
