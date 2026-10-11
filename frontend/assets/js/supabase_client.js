/**
 * This file sets up supabase connection. Import valiable 'supabase' to use. 
 */
import { createClient } from
    'https://esm.sh/@supabase/supabase-js@2';

const SUPABASE_URL =
    'https://wdxlljbnprencuzyentl.supabase.co';

const SUPABASE_PUBLISHABLE_KEY =
    'sb_publishable_b2OreJpe_3vzy5heaDvYSw_IzbGX8J_';

export const supabase = createClient(
    SUPABASE_URL,
    SUPABASE_PUBLISHABLE_KEY
);