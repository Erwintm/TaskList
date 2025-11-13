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
        { prefs.OrdenPrioridades[0], 0.5 },
        { prefs.OrdenPrioridades[1], 0.3 },
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
                            pFecha = PuntuarTiempoEntrega(tarea.FechaEntrega.Value);
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
