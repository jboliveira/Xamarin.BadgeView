# Xamarin Android - BadgeView

`BadgeView` component for `Xamarin.Android`.

## Usage

```csharp
var btnSample = FindViewById<ImageButton>(Resource.Id.btn_sample);
var badgeTarget = new BadgeView(this, btnSample);
badgeTarget.Text = "10";
badgeTarget.Show();
```
