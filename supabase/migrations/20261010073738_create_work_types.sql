CREATE TABLE public.work_types (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY
);

CREATE TABLE public.work_type_names (
    type_id BIGINT PRIMARY KEY
        REFERENCES public.work_types(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    en TEXT,
    ja TEXT
);

CREATE TABLE public.work_type_descriptions (
    type_id BIGINT PRIMARY KEY
        REFERENCES public.work_types(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    en TEXT,
    ja TEXT
);