export class History {
    private static _histories: History[] = [];

    public id: bigint;
    public titles: {
        en: string;
        ja: string;
    };
    public comments: {
        en: string;
        ja: string;
    }
    public type_id: bigint;
    public duration_minutes: number;
    public break_minutes: number;
    public started: Date;
    public uploaded: Date;
    public timezone: string; 

    public constructor(
        id: bigint,
        titleEn: string,
        titleJa: string,
        commentEn: string,
        commentJa: string,
        type_id: bigint,
        duration_minutes: number,
        break_minutes: number,
        started: Date,
        uploaded: Date,
        timezone: string
    ) {
        this.id = id;
        this.titles = {
            en: titleEn,
            ja: titleJa
        };
        this.comments = {
            en: commentEn,
            ja: commentJa
        };
        this.type_id = type_id;
        this.duration_minutes = duration_minutes;
        this.break_minutes = break_minutes;
        this.started = started;
        this.uploaded = uploaded;
        this.timezone = timezone;

        History._histories.push(this);
    };

    public static fromJson(json: any): History {

        return new History(
            BigInt(json.id),
            json.titles.en,
            json.titles.ja,
            json.comments.en,
            json.comments.ja,
            BigInt(json.type_id),
            json.duration_minutes,
            json.break_minutes,
            new Date(json.started),
            new Date(json.uploaded),
            json.timezone
        );
    }

    public static getHistory(id: bigint): History | undefined {
        return History._histories.find(history => history.id === id);
    }
}