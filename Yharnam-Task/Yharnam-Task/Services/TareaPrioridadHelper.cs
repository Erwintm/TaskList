using System;
using System.Collections.Generic;
using Yharnam_Task.Models;

namespace Yharnam_Task.Services
{
    public static class TareaPrioridadHelper
    {
        public static double CalcularPrioridad(Tarea tarea, ConfiguracionUsuario prefs)
        {
            System.Diagnostics.Debug.WriteLine($"Orden: {string.Join(", ", prefs.OrdenPrioridades)}");

            if (prefs == null || prefs.OrdenPrioridades == null || prefs.OrdenPrioridades.Count < 3)
                return 0;

            System.Diagnostics.Debug.WriteLine($"Orden: {string.Join(", ", prefs.OrdenPrioridades)}");

            var pesos = new Dictionary<string, double>
            {
                { prefs.OrdenPrioridades[0], 0.5 },
                { prefs.OrdenPrioridades[1], 0.3 },
                { prefs.OrdenPrioridades[2], 0.2 }
            };

            double puntajeTotal = 0;

            foreach (var (clave, peso) in pesos)
            {
                switch (clave)
                {
                    case "Dificultad":
                        puntajeTotal += peso * PuntuarDificultad(tarea.Dificultad, prefs.PreferenciaDificultad);
                        break;

                    case "Tiempo de entrega":
                        if (tarea.FechaEntrega.HasValue)
                            puntajeTotal += peso * PuntuarTiempoEntrega(tarea.FechaEntrega.Value);
                        break;

                    case "Duracion":
                        if (tarea.TiempoEstimado.HasValue)
                            puntajeTotal += peso * PuntuarDuracion(tarea.TiempoEstimado.Value, prefs.PreferenciaDuracion);
                        break;
                }
            }

            return puntajeTotal * 100;
        }

        private static double PuntuarDificultad(string dificultad, string? preferenciaUsuario)
        {
            double valor = dificultad switch
            {
                "Fácil" => 0.3,
                "Media" => 0.6,
                "Difícil" => 1.0,
                _ => 0
            };

            if (!string.IsNullOrEmpty(preferenciaUsuario) &&
                dificultad.Equals(preferenciaUsuario, StringComparison.OrdinalIgnoreCase))
                valor += 0.1;

            return Math.Min(valor, 1.0);
        }

        private static double PuntuarTiempoEntrega(DateTime fechaEntrega)
        {
            var diasRestantes = (fechaEntrega - DateTime.Today).TotalDays;

            if (diasRestantes <= 1) return 1.0;   
            if (diasRestantes <= 3) return 0.7;
            if (diasRestantes <= 7) return 0.4;
            return 0.2; 
        }

        private static double PuntuarDuracion(TimeSpan duracion, string? preferenciaUsuario)
        {
            double minutos = duracion.TotalMinutes;
            double valor = minutos switch
            {
                <= 30 => 0.3,
                <= 120 => 0.6,
                _ => 1.0
            };

            if (!string.IsNullOrEmpty(preferenciaUsuario))
            {
                if ((preferenciaUsuario == "Corta" && minutos <= 30) ||
                    (preferenciaUsuario == "Media" && minutos <= 120) ||
                    (preferenciaUsuario == "Larga" && minutos > 120))
                    valor += 0.1;
            }

            return Math.Min(valor, 1.0);
        }
    }
}
