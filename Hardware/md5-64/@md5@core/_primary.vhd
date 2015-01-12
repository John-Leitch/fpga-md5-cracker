library verilog;
use verilog.vl_types.all;
entity Md5Core is
    port(
        clk             : in     vl_logic;
        wb              : in     vl_logic_vector(511 downto 0);
        a0              : in     vl_logic_vector(31 downto 0);
        b0              : in     vl_logic_vector(31 downto 0);
        c0              : in     vl_logic_vector(31 downto 0);
        d0              : in     vl_logic_vector(31 downto 0);
        a64             : out    vl_logic_vector(31 downto 0);
        b64             : out    vl_logic_vector(31 downto 0);
        c64             : out    vl_logic_vector(31 downto 0);
        d64             : out    vl_logic_vector(31 downto 0)
    );
end Md5Core;
