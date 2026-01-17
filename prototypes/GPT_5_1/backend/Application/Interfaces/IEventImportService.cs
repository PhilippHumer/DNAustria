namespace Application.Interfaces;

public interface IEventImportService
{
    /// <summary>
    /// Importiert mehrere Events aus einem JSON Array (export-event Schema) und legt fehlende Organisationen/Kontakte an.
    /// Gibt die Anzahl neu angelegter Events zurück.
    /// </summary>
    /// <param name="json">JSON Array von Objekten im Format export-sample.json</param>
    Task<int> ImportAsync(string json, CancellationToken ct = default);
}
