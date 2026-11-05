export class WorkType {
    private static _workTypes: WorkType[] = [];
    
    public id: bigint;
    public names: {
        en: string;
        jn: string;
    }
    public descriptions: {
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
        this.names = {
            en: nameEn,
            jn: nameJn
        };
        this.descriptions = {
            en: descriptionEn,
            jn: descriptionJn
        };

        WorkType._workTypes.push(this);
    }

    public static fromJson(json: any): WorkType {
        return new WorkType(
            BigInt(json.id),
            json.work_type_names.en,
            json.work_type_names.jn,
            json.work_type_descriptions.en,
            json.work_type_descriptions.jn
        );
    }

    public static getWorkType(id: bigint): WorkType | undefined {
        return WorkType._workTypes.find(workType => workType.id === id);
    }

    public static getAllWorkTypes(): WorkType[] {
        return WorkType._workTypes;
    }
}