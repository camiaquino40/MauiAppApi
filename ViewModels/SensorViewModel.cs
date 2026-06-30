using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiApiApp.ViewModels
{
    public partial class SensorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string ubicacion = "Sin datos";

        [ObservableProperty]
        private string acelerometro = "Sin datos";

        [ObservableProperty]
        private string estadoCamara = "No iniciada";

        [ObservableProperty]
        private string textoBotonAcelerometro = "Iniciar acelerómetro";

        [ObservableProperty]
        private bool acelerometroActivo;

        // ─── GPS ───────────────────────────────────────────────────────────────

        [RelayCommand]
        private async Task ObtenerUbicacionAsync()
        {
            try
            {
                var permiso = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (permiso != PermissionStatus.Granted)
                {
                    Ubicacion = "Permiso de ubicación denegado";
                    return;
                }

                var loc = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                Ubicacion = loc is not null
                    ? $"Lat: {loc.Latitude:F5} | Lon: {loc.Longitude:F5}"
                    : "No se pudo obtener la ubicación";
            }
            catch (FeatureNotSupportedException)
            {
                Ubicacion = "GPS no disponible en este dispositivo";
            }
            catch (Exception ex)
            {
                Ubicacion = $"Error: {ex.Message}";
            }
        }

        // ─── Acelerómetro ──────────────────────────────────────────────────────

        [RelayCommand]
        private void ToggleAcelerometro()
        {
            if (!AcelerometroActivo)
            {
                if (!Accelerometer.Default.IsSupported)
                {
                    Acelerometro = "No disponible en este dispositivo";
                    return;
                }

                Accelerometer.Default.ReadingChanged += OnAccelerometerChanged;
                Accelerometer.Default.Start(SensorSpeed.UI);
                AcelerometroActivo = true;
                TextoBotonAcelerometro = "Detener acelerómetro";
            }
            else
            {
                Accelerometer.Default.Stop();
                Accelerometer.Default.ReadingChanged -= OnAccelerometerChanged;
                AcelerometroActivo = false;
                Acelerometro = "Detenido";
                TextoBotonAcelerometro = "Iniciar acelerómetro";
            }
        }

        private void OnAccelerometerChanged(object? sender, AccelerometerChangedEventArgs e)
        {
            var d = e.Reading.Acceleration;
            Acelerometro = $"X: {d.X:F2} | Y: {d.Y:F2} | Z: {d.Z:F2}";
        }

        // ─── Cámara ────────────────────────────────────────────────────────────

        [RelayCommand]
        private async Task TomarFotoAsync()
        {
            try
            {
                var permiso = await Permissions.RequestAsync<Permissions.Camera>();
                if (permiso != PermissionStatus.Granted)
                {
                    EstadoCamara = "Permiso de cámara denegado";
                    return;
                }

                var foto = await MediaPicker.Default.CapturePhotoAsync();
                EstadoCamara = foto is not null
                    ? $"Foto guardada: {foto.FileName}"
                    : "Cancelado";
            }
            catch (FeatureNotSupportedException)
            {
                EstadoCamara = "Cámara no disponible";
            }
            catch (Exception ex)
            {
                EstadoCamara = $"Error: {ex.Message}";
            }
        }

        // ─── Vibración ─────────────────────────────────────────────────────────

        [RelayCommand]
        private void Vibrar()
        {
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(400));
            }
            catch (FeatureNotSupportedException)
            {
                // algunos emuladores no soportan vibración
            }
        }
    }
}
