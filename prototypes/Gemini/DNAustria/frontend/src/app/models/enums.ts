export enum TargetAudience {
    VorschulkinderElementarstufe = 10,
    SchulkinderPrimarstufe = 20,
    JugendlicheSekundarstufeI = 30,
    JugendlicheBerufsschulenPTS = 40,
    JugendlicheSekundarstufeII = 50,
    Erwachsene = 60,
    Familien = 70,
    NurMaedchenFrauen = 80
}

export enum EventTopic {
    DigitalisierungKuenstlicheIntelligenzITTechnik = 100,
    KunstKultur = 200,
    SprachenLiteratur = 300,
    MedizinGesundheit = 400,
    GeschichteDemokratieGesellschaft = 500,
    WirtschaftRecht = 600,
    NaturwissenschaftKlimaUmwelt = 700,
    MathematikZahlenDaten = 800
}

export enum EventStatus {
    Draft = 0,
    Approved = 1,
    Transferred = 2
}

export enum EventClassification {
    Scheduled = 0,
    OnDemand = 1
}
