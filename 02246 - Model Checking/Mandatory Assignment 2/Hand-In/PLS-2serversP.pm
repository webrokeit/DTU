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
  ticket1 : [0..5] init 0; // Number of tickets for job1
  ticket2 : [0..5] init 0; // Number of tickets for job2
  ticket3 : [0..5] init 0; // Number of tickets for job3
  useserver : [0..2] init 0;

  // Record that there is a waiting job
  [create1] job1=false -> (job1'=true) & (ticket1'=5-task1);
  [create2] job2=false -> (job2'=true) & (ticket2'=5-task2);
  [create3] job3=false -> (job3'=true) & (ticket3'=5-task3);


  // Serve the jobs
  [] useserver=0 -> 0.5:(useserver'=1) + 0.5:(useserver'=2);

  [server1job1] job1=true & ticket1>=ticket2 & ticket1>=ticket3 & useserver=1 -> (useserver'=0);
  [server1job2] job2=true & ticket2>=ticket1 & ticket2>=ticket3 & useserver=1 -> (useserver'=0);
  [server1job3] job3=true & ticket3>=ticket1 & ticket3>=ticket2 & useserver=1 -> (useserver'=0);
 
  [server2job1] job1=true & ticket1>=ticket2 & ticket1>=ticket3 & useserver=2 -> (useserver'=0);
  [server2job2] job2=true & ticket2>=ticket1 & ticket2>=ticket3 & useserver=2 -> (useserver'=0);
  [server2job3] job3=true & ticket3>=ticket1 & ticket3>=ticket2 & useserver=2 -> (useserver'=0);

  // Complete any job that has finished
  [finish1] job1=true -> (job1'=false) & (ticket1'=0);
  [finish2] job2=true -> (job2'=false) & (ticket2'=0);
  [finish3] job3=true -> (job3'=false) & (ticket3'=0);

endmodule

module server1
  jobserving : [0..3] init 0;
  serving : bool init false;

  [] serving=true -> (jobserving'=0) & (serving'=false); 

  [serve1] jobserving=1 -> (serving'=true);
  [serve2] jobserving=2 -> (serving'=true);
  [serve3] jobserving=3 -> (serving'=true);

  [server1job1] serving=false -> (jobserving'=1);
  [server1job2] serving=false -> (jobserving'=2);
  [server1job3] serving=false -> (jobserving'=3);

endmodule

module server2
  servejob : [0..3] init 0;
  toserve : bool init false;

  [] toserve=true -> (servejob'=0) & (toserve'=false);

  [serve1] servejob=1 -> (toserve'=true);
  [serve2] servejob=2 -> (toserve'=true);
  [serve3] servejob=3 -> (toserve'=true);

  [server2job1] toserve=false -> (servejob'=1);
  [server2job2] toserve=false -> (servejob'=2);
  [server2job3] toserve=false -> (servejob'=3);

endmodule


module monitor
  finished : bool init false;

  [finish1] finished=false -> (finished'=true);
  [finish1] finished=true -> (finished'=true);

endmodule

system
  scheduler || client1 || client2 || client3 || monitor || server1 || server2
endsystem

