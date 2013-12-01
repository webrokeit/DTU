dtmc

module figure
  s : [0..3] init 0;
  
  [] s = 0 -> 1 : (s' = 2);
  [] s = 1 -> 0.2 : (s' = 0) + 0.8 : (s' = 1);
  [] s = 2 -> 0.3 : (s' = 0) + 0.7 : (s' = 1);
endmodule

label "a" = s=0 | s=2;
label "b" = s=1;