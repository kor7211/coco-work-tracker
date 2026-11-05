export class WorkType {
    private static _workTypes: WorkType[] = [];
    
    public id: bigint;
    public name: {
        en: string;
        jn: string;
    }
    public description: {
        en: string;
        jn: string;
    }

    public constructor(
        id: bigint,
        nameEn: string,
        nameJn: string,
        descriptionEn: string,
        descriptionJn: string,
    ) {
        this.id = id;
        this.name = {
            en: nameEn,
            jn: nameJn
        };
        this.description = {
            en: descriptionEn,
            jn: descriptionJn
        };

        WorkType._workTypes.push(this);
    }

    public static fromJson(json: any): WorkType {
        return new WorkType(
            BigInt(json.id),
            json.name.en,
            json.name.jn,
            json.description.en,
            json.description.jn
        );
    }

    public static getWorkType(id: bigint): WorkType | undefined {
        return WorkType._workTypes.find(workType => workType.id === id);
    }

    public static getAllWorkTypes(): WorkType[] {
        return WorkType._workTypes;
    }
}