dtmc

module figure
  s : [0..5] init 0;
  
  [] s = 0 -> 1 : (s' = 3);
  [] s = 1 -> 0.1 : (s' = 1) + 0.2 : (s' = 2) + 0.7 : (s' = 4);
  [] s = 2 -> 0.5 : (s' = 1) + 0.2 : (s' = 4) + 0.3 : (s' = 5);
  [] s = 3 -> 0.3 : (s' = 1) + 0.7 : (s' = 4);
  [] s = 4 -> 0.2 : (s' = 0) + 0.8 : (s' = 1);
  [] s = 5 -> 1.0 : (s' = 2);
endmodule

label "a" = s=0 | s=2 | s=3 | s=5;
label "b" = s=1 | s=4;