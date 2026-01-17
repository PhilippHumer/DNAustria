namespace DNAustria.Backend.Models;

public enum Classification
{
    Scheduled,
    OnDemand
}

public enum EventStatus
{
    Draft,
    Approved,
    Transferred
}

public enum TargetAudience
{
    Vorschulkinder = 10,
    Schulkinder = 20,
    Jugendliche_SekI = 30,
    Jugendliche_Berufsschulen = 40,
    Jugendliche_SekII = 50,
    Erwachsene = 60,
    Familien = 70,
    Nur_Maedchen_Frauen = 80
}

public enum EventTopic
{
    Digitalisierung_IT = 100,
    Kunst_Kultur = 200,
    Sprachen_Literatur = 300,
    Medizin_Gesundheit = 400,
    Geschichte_Demokratie_Gesellschaft = 500,
    Wirtschaft_Recht = 600,
    Naturwissenschaft_Klima_Umwelt = 700,
    Mathematik_Zahlen_Daten = 800
}