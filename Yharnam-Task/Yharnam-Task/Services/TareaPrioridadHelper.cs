using System;
using System.Collections.Generic;
using Yharnam_Task.Models;

namespace Yharnam_Task.Services
{
    public static class TareaPrioridadHelper
    {
        public static double CalcularPrioridad(Tarea tarea, ConfiguracionUsuario prefs)
        {
            if (prefs == null || prefs.OrdenPrioridades == null || prefs.OrdenPrioridades.Count < 3)
                return 0;

            var pesos = new Dictionary<string, double>
    {
        { prefs.OrdenPrioridades[0], 1 },
        { prefs.OrdenPrioridades[1], 0.5 },
        { prefs.OrdenPrioridades[2], 0.2 }
    };

            double puntajeTotal = 0;
            double pDificultad = 0, pDuracion = 0, pFecha = 0;

            foreach (var (clave, peso) in pesos)
            {
                switch (clave)
                {
                    case "Dificultad":
                        pDificultad = PuntuarDificultad(tarea.Dificultad, prefs.PreferenciaDificultad);
                        puntajeTotal += peso * pDificultad;
                        break;

                    case "Tiempo de entrega":
                        if (tarea.FechaEntrega.HasValue)
                        {
                            pFecha = PuntuarTiempoEntrega(tarea.FechaEntrega.Value, prefs.PreferenciaPrioridad);
                            puntajeTotal += peso * pFecha;
                        }
                        break;

                    case "Duracion":
                        if (tarea.TiempoEstimado.HasValue)
                        {
                            pDuracion = PuntuarDuracion(tarea.TiempoEstimado.Value, prefs.PreferenciaDuracion);
                            puntajeTotal += peso * pDuracion;
                        }
                        break;
                }
            }

            tarea.PrioridadDificultad = pDificultad;
            tarea.PrioridadDuracion = pDuracion;
            tarea.PrioridadFecha = pFecha;
            tarea.PrioridadCalculada = puntajeTotal * 100;

            return tarea.PrioridadCalculada;
        }

        private static double PuntuarDificultad(string dificultad, string? preferenciaUsuario)
        {
            double valor = dificultad switch
            {
                "Fácil" => 0.1,
                "Media" => 0.3,
                "Difícil" => 0.5,
                _ => 0
            };

            if (!string.IsNullOrEmpty(preferenciaUsuario) && NormalizarTexto(dificultad).Equals(NormalizarTexto(preferenciaUsuario), 
                StringComparison.OrdinalIgnoreCase))
                valor += 0.5;

            return valor;
        }

        private static double PuntuarTiempoEntrega(DateTime fechaEntrega, string? preferenciaUsuario)
        {
            var diasRestantes = (fechaEntrega - DateTime.Today).TotalDays;
            double valor;

            if (diasRestantes <= 1) valor = 0.4;
            else if (diasRestantes <= 3) valor = 0.3;
            else if (diasRestantes <= 7) valor = 0.2;
            else valor = 0.1;

            if (!string.IsNullOrEmpty(preferenciaUsuario))
            {
                if ((preferenciaUsuario.Equals("Alta", StringComparison.OrdinalIgnoreCase) && diasRestantes <= 1) ||
                    (preferenciaUsuario.Equals("Media", StringComparison.OrdinalIgnoreCase) && diasRestantes <= 3) ||
                    (preferenciaUsuario.Equals("Baja", StringComparison.OrdinalIgnoreCase) && diasRestantes > 7))
                {
                    valor += 0.7;
                }
            }

            return valor;
        }

        private static double PuntuarDuracion(TimeSpan duracion, string? preferenciaUsuario)
        {
            double minutos = duracion.TotalMinutes;
            double valor = minutos switch
            {
                <= 30 => 0.1,
                <= 120 => 0.3,
                _ => 0.5
            };

            if (!string.IsNullOrEmpty(preferenciaUsuario))
            {
                if ((preferenciaUsuario == "Corta" && minutos <= 30) ||
                    (preferenciaUsuario == "Media" && minutos <= 120) ||
                    (preferenciaUsuario == "Larga" && minutos > 120))
                    valor += 0.7; 
            }

            return valor;
        }
        private static string NormalizarTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;

            var normalized = texto.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

    }
}
