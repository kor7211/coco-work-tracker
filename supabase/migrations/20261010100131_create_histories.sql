-- =========================================
-- Histories
-- =========================================
CREATE TABLE public.histories (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id BIGINT NOT NULL
        REFERENCES public.work_types(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    timezone TEXT NOT NULL,
    uploaded TIMESTAMPTZ NOT NULL,
    started TIMESTAMPTZ NOT NULL,
    duration_minutes INTEGER NOT NULL,
    break_minutes INTEGER NOT NULL,

    CONSTRAINT histories_duration_nonnegative
        CHECK (duration_minutes >= 0),

    CONSTRAINT histories_break_nonnegative
        CHECK (break_minutes >= 0)
);


-- =========================================
-- Titles
-- =========================================
CREATE TABLE public.titles (
    history_id BIGINT PRIMARY KEY
        REFERENCES public.histories(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    en TEXT,
    ja TEXT
);


-- =========================================
-- Comments
-- =========================================
CREATE TABLE public.comments (
    history_id BIGINT PRIMARY KEY
        REFERENCES public.histories(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    en TEXT,
    ja TEXT
);


-- =========================================
-- Images
-- =========================================
CREATE TABLE public.images (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,

    history_id BIGINT NOT NULL
        REFERENCES public.histories(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    path TEXT NOT NULL
);


-- =========================================
-- Indexes
-- =========================================
CREATE INDEX idx_histories_started_id
    ON public.histories (started DESC, id DESC);

CREATE INDEX idx_histories_type_id
    ON public.histories (type_id);

CREATE INDEX idx_images_history_id
    ON public.images (history_id);
