dtmc

module client1
  state1 : [0..1] init 0; // State of the job (inactive/active)
  task1  : [0..5] init 0; // Length of the job
  
  [create1] state1=0 -> 0.2:(state1'=1) & (task1'=1) + 0.2 :(state1'=1) & (task1'=2) + 0.2 :(state1'=1) & (task1'=3) + 0.2: (state1'=1) & (task1'=4) + 0.2:(state1'=1) & (task1'=5);

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

module client3 = client1 [state1=state3,
			  task1=task3,
			  create1=create3,
			  serve1=serve3,
			  finish1=finish3 ]
endmodule

module scheduler
  job1 : bool init false; // Is there a job from client1?
  job2 : bool init false; // Is there a job from client2?
  job3 : bool init false; // Is there a job from client3?
  ticket : [0..3] init 0; // The tickets
  time : [0..2] init 0; // The time units


  // Record that there is a waiting job
  [create1] job1=false -> (job1'=true);
  [create2] job2=false -> (job2'=true);
  [create3] job3=false -> (job3'=true);

  [] ticket=0 -> 0.333333333333333333333333:(ticket'=1) & (time'=2) + 0.333333333333333333333333:(ticket'=2) & (time'=2) + 0.333333333333333333333333:(ticket'=3) & (time'=2);
  [] time=0 & ticket>0 -> (ticket'=0);

  // Serve the jobs
  [serve1] job1=true & ticket=1 & time>0 -> (time'=time-1);
  [serve2] job2=true & ticket=2 & time>0 -> (time'=time-1);
  [serve3] job3=true & ticket=3 & time>0 -> (time'=time-1);    

  // Complete any job that has finished
  [finish1] job1=true -> (job1'=false) & (time'=0);
  [finish2] job2=true -> (job2'=false) & (time'=0);
  [finish3] job3=true -> (job3'=false) & (time'=0);

endmodule

module monitor
  finished : bool init false;

  [finish1] finished=false -> (finished'=true);
  [finish1] finished=true -> (finished'=true);

endmodule

system
  scheduler || client1 || client2 || client3 || monitor
endsystem

