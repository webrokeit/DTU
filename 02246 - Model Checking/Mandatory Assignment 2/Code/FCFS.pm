dtmc

module client1
  state1 : [0..1] init 0; // State of the job (inactive/active)
  task1  : [0..5] init 0; // Length of the job
  
  [create1] state1=0 -> 0.2 :(state1'=1) & (task1'=1) + 0.2 :(state1'=1) & (task1'=2) + 0.2 :(state1'=1) & (task1'=3) + 0.2 :(state1'=1) & (task1'=4) + 0.2 :(state1'=1) & (task1'=5);

  // Serve the job
  [serve1] state1=1 & task1>0 -> (task1'=task1-1);

  // Complete the job
  [finish1] state1=1 & task1=0 -> (state1'=0);

endmodule

module client2 = client1 [state1=state2,
                          task1=task2,
                          create1=create2,
                          serve1=serve2,
                          finish1=finish2 ]
endmodule

module scheduler
  job1 : [0..2] init 0; // First job in the queue
  job2 : [0..2] init 0; // Second job in the queue

  // Place a new job at the end of the queue
  [create1] job2=0 -> (job2'=1);
  [create2] job2=0 -> (job2'=2);

  // Shift the queue if there is an empty slot
  [] job1=0 & job2>0 -> (job1'=job2) & (job2'=0);

  // Serve the job at the head of the queue
  [serve1] job1=1 -> true;
  [serve2] job1=2 -> true;

  // Complete the job at the head of the queue
  [finish1] job1=1 -> (job1'=0);
  [finish2] job1=2 -> (job1'=0);

endmodule

system
  scheduler || client1 || client2
endsystem

